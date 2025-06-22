using System.Web;
using AppLaunch.Models;
using Applaunch.Models.Files;
using AppLaunch.Services.Data;
using Microsoft.EntityFrameworkCore;
using File = AppLaunch.Services.Data.File;

namespace AppLaunch.Services;

public interface IFileService
{
    Task<CoreResponse<List<FileDto>>> GetAllFiles();
    Task<CoreResponse<FileDto>> GetFile(FileRequestDto model);
    Task<CoreResponse> DeleteFile(FileRequestDto model);
    Task<CoreResponse<FileDto>> SaveFile(FileSaveDto model);
    Task<CoreResponse<FileDataDto>> GetFileBytes(FileRequestDto model);
    Task<CoreResponse<List<FileCategoryDto>>> GetAllCategories();
}

public class FileService(IDbContextFactory<ApplicationDbContext> contextFactory) : IFileService
{
    public async Task<CoreResponse<List<FileDto>>> GetAllFiles()
    {
        CoreResponse<List<FileDto>> myResponse = new();
        try
        {
            using var context = contextFactory.CreateDbContext();
            List<FileDto> myFiles = new();
            
            var dbFiles = await (from s in context.Files
                select new 
                {
                    s.FileId,
                    s.FileName,
                    s.Category,
                    s.MimeType
                }).ToListAsync();

            foreach (var file in dbFiles)
            {
                FileDto myFile = new()
                {
                    FileId = file.FileId,
                    FileName = file.FileName,
                    MimeType = file.MimeType,
                    Category = file.Category,
                    FileUrl = $"/_files/{file.FileId.ToString()}/{file.FileName}"
                };
                myFiles.Add(myFile);
            }
            myResponse.Data = myFiles;
            myResponse.IsSuccess = true;
        }
        catch (Exception ex)
        {
            myResponse.IsSuccess = false;
            myResponse.Message = ex.Message;
        }
        return myResponse;
    }
    
    public async Task<CoreResponse<List<FileCategoryDto>>> GetAllCategories()
    {
        CoreResponse<List<FileCategoryDto>> myResponse = new();
        try
        {
            using var context = contextFactory.CreateDbContext();
            List<FileCategoryDto> myCategories = new();
            
            var dbCategories = await (from s in context.Files
                orderby s.Category
                select new {s.Category}).Distinct().ToListAsync();

            foreach (var category in dbCategories)
            {
                FileCategoryDto myCategory = new()
                {
                    Category = category.Category,
                };
                myCategories.Add(myCategory);
            }
            myResponse.Data = myCategories;
            myResponse.IsSuccess = true;
        }
        catch (Exception ex)
        {
            myResponse.IsSuccess = false;
            myResponse.Message = ex.Message;
        }
        return myResponse;
    }
    
    public async Task<CoreResponse<FileDto>> GetFile(FileRequestDto model)
    {
        CoreResponse<FileDto> myResponse = new();
        try
        {
            using var context = contextFactory.CreateDbContext();
            var dbFile = await (from s in context.Files
                where s.FileId == model.FileId
                select new 
                {
                    s.FileId,
                    s.FileName,
                    s.Category,
                    s.MimeType
                }).FirstOrDefaultAsync();
            
            if (dbFile == null) throw new Exception("File not found");
                FileDto myFile = new()
                {
                    FileId = dbFile.FileId,
                    FileName = dbFile.FileName,
                    MimeType = dbFile.MimeType,
                    Category = dbFile.Category,
                    FileUrl = $"/_files/{dbFile.FileId.ToString()}/{HttpUtility.UrlEncode(dbFile.FileName)}"
                };
            myResponse.Data = myFile;
            myResponse.IsSuccess = true;
        }
        catch (Exception ex)
        {
            myResponse.IsSuccess = false;
            myResponse.Message = ex.Message;
        }
        return myResponse;
    }
    
    public async Task<CoreResponse<FileDataDto>> GetFileBytes(FileRequestDto model)
    {
        CoreResponse<FileDataDto> myResponse = new();
        try
        {
            using var context = contextFactory.CreateDbContext();
            var dbFile = await (from s in context.Files
                where s.FileId == model.FileId
                select new 
                {
                    s.FileId,
                    s.FileName,
                    s.Category,
                    s.MimeType,
                    s.FileBytes
                }).FirstOrDefaultAsync();
            
            if (dbFile == null) throw new Exception("File not found");
            FileDataDto myFile = new()
            {
                FileId = dbFile.FileId,
                FileName = dbFile.FileName,
                MimeType = dbFile.MimeType,
                Category = dbFile.Category,
                FileBytes = dbFile.FileBytes,
                FileUrl = $"/_files/{dbFile.FileId.ToString()}/{dbFile.FileName}"
            };
            myResponse.Data = myFile;
            myResponse.IsSuccess = true;
        }
        catch (Exception ex)
        {
            myResponse.IsSuccess = false;
            myResponse.Message = ex.Message;
        }
        return myResponse;
    }
    
    public async Task<CoreResponse> DeleteFile(FileRequestDto model)
    {
        CoreResponse myResponse = new();
        try
        {
            using var context = contextFactory.CreateDbContext();
            var dbFile = await (from f in context.Files
                where f.FileId == model.FileId
                select f).FirstOrDefaultAsync();
            if (dbFile == null) throw new Exception("File not found");
            context.Files.Attach(dbFile);
            context.Files.Remove(dbFile);
            await context.SaveChangesAsync();
            myResponse.IsSuccess = true;
        }
        catch (Exception ex)
        {
            myResponse.IsSuccess = false;
            myResponse.Message = ex.Message;
        }
        return myResponse;
    }
    
    public async Task<CoreResponse<FileDto>> SaveFile(FileSaveDto model)
    {
        CoreResponse<FileDto> myResponse = new();
        try
        {
            using var context = contextFactory.CreateDbContext();
            File dbFile = new();
            if (model.FileId == Guid.Empty) model.FileId = Guid.NewGuid();

            var existingFile = await context.Files
                .Where(f => f.FileId == model.FileId)
                .FirstOrDefaultAsync();

            if (existingFile == null)
            {
                context.Files.Add(dbFile);
            }
            else
            {
                dbFile = existingFile;
                context.Files.Attach(dbFile).State = EntityState.Modified;
            }
            
            dbFile.FileId = model.FileId;
            dbFile.FileName = model.FileName;
            dbFile.Category = model.Category;
            dbFile.MimeType =  await GetMimeType(model.FileName);
            dbFile.FileBytes = model.FileBytes;
            await context.SaveChangesAsync();
            
            //get file
            FileRequestDto fileRequest = new()
            {
                FileId = model.FileId
            };
            var fileResponse=await GetFile(fileRequest);
            if (!fileResponse.IsSuccess) throw new Exception(fileResponse.Message);
            
            myResponse.IsSuccess = true;
            myResponse.Data = fileResponse.Data;
        }
        catch (Exception ex)
        {
            myResponse.IsSuccess = false;
            myResponse.Message = ex.Message;
        }
        return myResponse;
    }
    
    private async Task<string> GetFileExtension(string Filename)
    {
        try
        {
            string extension = Path.GetExtension(Filename);
            if (string.IsNullOrEmpty(extension))
            {
                return "";
            }
            else
            {
                return extension;
            }
        }
        catch (Exception ex)
        {
            return "";
        }
    }
    
    public async Task<string> GetMimeType(string FileName)
    {
        try
        {
            var FileExtension=await GetFileExtension(FileName.ToLower());
            if (FileExtension == "") throw new Exception("No Extension");
            string mimeType = MimeTypes.TryGetValue(FileExtension, out string type) ? type : "application/octet-stream";
            return mimeType;
        }
        catch (Exception ex)
        {
            return "application/octet-stream";
        }
    }
    
    private static readonly Dictionary<string, string> MimeTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
      { ".txt", "text/plain" },
      { ".pdf", "application/pdf" },
      { ".doc", "application/msword" },
      { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
      { ".xls", "application/vnd.ms-excel" },
      { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
      { ".png", "image/png" },
      { ".jpg", "image/jpeg" },
      { ".jpeg", "image/jpeg" },
      { ".gif", "image/gif" },
      { ".csv", "text/csv" },
      { ".html", "text/html" },
      { ".htm", "text/html" },
      { ".json", "application/json" },
      { ".xml", "application/xml" },
      { ".webp", "image/webp" },
      { ".svg", "image/svg+xml" },
      { ".zip", "application/zip" },
      { ".mp3", "audio/mpeg" },
      { ".wav", "audio/wav" },
      { ".ogg", "audio/ogg" },
      { ".mp4", "video/mp4" },
      { ".avi", "video/x-msvideo" },
      { ".mpeg", "video/mpeg" },
      { ".mov", "video/quicktime" },
      { ".webm", "video/webm" },
      { ".flac", "audio/flac" },
      { ".bmp", "image/bmp" },
      { ".tif", "image/tiff" },
      { ".tiff", "image/tiff" },
      { ".ico", "image/vnd.microsoft.icon" },
      { ".psd", "image/vnd.adobe.photoshop" },
      { ".eps", "application/postscript" },
      { ".ai", "application/postscript" },
      { ".rtf", "application/rtf" },
      { ".midi", "audio/midi" },
      { ".m4a", "audio/x-m4a" },
      { ".wmv", "video/x-ms-wmv" },
      { ".flv", "video/x-flv" },
      { ".mkv", "video/x-matroska" },
      { ".7z", "application/x-7z-compressed" },
      { ".rar", "application/vnd.rar" },
      { ".tar", "application/x-tar" },
      { ".gz", "application/gzip" },
      { ".ppt", "application/vnd.ms-powerpoint" },
      { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
      { ".odt", "application/vnd.oasis.opendocument.text" },
      { ".ods", "application/vnd.oasis.opendocument.spreadsheet" },
      { ".odp", "application/vnd.oasis.opendocument.presentation" }
    };
}