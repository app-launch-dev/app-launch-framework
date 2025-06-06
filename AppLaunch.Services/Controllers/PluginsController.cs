using System.IO.Compression;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppLaunch.Services.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PluginsController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public PluginsController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpPost("install")]
    public async Task<IActionResult> InstallPlugin(IFormFile package)
    {
        const string pluginsDir = "PluginData";
        var extractPath = Path.Combine(pluginsDir, Guid.NewGuid().ToString());
        var nupkgPath = Path.Combine(pluginsDir, package.FileName);

        try
        {
            // 1. Validate package
            if (!package.FileName.EndsWith(".nupkg", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid NuGet package format");

            // 2. Save uploaded package
            Directory.CreateDirectory(pluginsDir);
            await using (var stream = new FileStream(nupkgPath, FileMode.Create))
            {
                await package.CopyToAsync(stream);
            }

            // 3. Validate NuGet package structure
            using var archive = ZipFile.OpenRead(nupkgPath);
            if (!archive.Entries.Any(e => e.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase)))
                return BadRequest("Invalid NuGet package structure");

            // 4. Extract package
            Directory.CreateDirectory(extractPath);
            ZipFile.ExtractToDirectory(nupkgPath, extractPath);

            // 5. Cleanup nupkg
            System.IO.File.Delete(nupkgPath);

            return Ok(new { 
                ExtractPath = extractPath,
                AssemblyPath = Path.Combine(extractPath, "lib", "net9.0") 
            });
        }
        catch (Exception ex)
        {
            // Cleanup failed installation
            if (Directory.Exists(extractPath))
                Directory.Delete(extractPath, recursive: true);
            if (System.IO.File.Exists(nupkgPath))
                System.IO.File.Delete(nupkgPath);

            return Problem($"Package installation failed: {ex.Message}");
        }
    }

}
