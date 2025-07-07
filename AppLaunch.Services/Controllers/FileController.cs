using AppLaunch.Models;
using Applaunch.Models.Files;
using AppLaunch.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppLaunch.Services.Controllers;

public class FileController(IFileService FileService) : Controller
{
    [HttpGet("/_files/{FileId}/{Filename}")]
    public async Task<IActionResult> DownloadFile(string FileId, string Filename)
    {
        try
        {
            FileRequestDto fileRequest = new()
            {
                FileId = new Guid(FileId)
            };
            var fileResponse = await FileService.GetFileBytes(fileRequest);
            if (!fileResponse.IsSuccess) throw new Exception(fileResponse.Message);
            
            // Set Content-Disposition based on MIME type
            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = Filename,
                Inline = fileResponse.Data.MimeType.StartsWith("image/") || fileResponse.Data.MimeType == "application/pdf"
            };
            
            // Set Cache-Control header for images
            if (fileResponse.Data.MimeType.StartsWith("image/"))
            {
                const int cacheDurationInSeconds = 86400; // 1 day
                Response.Headers["Cache-Control"] = $"public,max-age={cacheDurationInSeconds}";
            }

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(fileResponse.Data.FileBytes, fileResponse.Data.MimeType);
            
        }
        catch (Exception ex)
        {
            if (ex.Message == "File not found")
            {
                return NotFound(new { message = ex.Message });
            }
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}