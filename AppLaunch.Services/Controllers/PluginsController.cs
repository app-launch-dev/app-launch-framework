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
        var guidFolder = Path.Combine(pluginsDir, Guid.NewGuid().ToString()); // Create GUID folder
        //var extractPath = Path.Combine(pluginsDir, Guid.NewGuid().ToString());
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
            Directory.CreateDirectory(guidFolder);
            ZipFile.ExtractToDirectory(nupkgPath, guidFolder);
            
            // Find the DLL file inside `lib/net9.0`
            var libFolder = Path.Combine(guidFolder, "lib", "net9.0");
            var dllFile = Directory.GetFiles(libFolder, "*.dll").FirstOrDefault();

            if (dllFile == null)
                return BadRequest("No DLL found inside extracted package.");

            var assemblyName = Path.GetFileNameWithoutExtension(dllFile); // Remove `.dll` extension
            var renamedFolder = Path.Combine(pluginsDir, assemblyName);

            // Rename the GUID-based folder to match the assembly name
            Directory.Move(guidFolder, renamedFolder);

            // 5. Cleanup nupkg
            System.IO.File.Delete(nupkgPath);

            return Ok(new { ExtractPath = renamedFolder, AssemblyName = assemblyName });
        }
        catch (Exception ex)
        {
            // Cleanup failed installation
            if (Directory.Exists(guidFolder))
                Directory.Delete(guidFolder, recursive: true);
            if (System.IO.File.Exists(nupkgPath))
                System.IO.File.Delete(nupkgPath);

            return Problem($"Package installation failed: {ex.Message}");
        }
    }

}
