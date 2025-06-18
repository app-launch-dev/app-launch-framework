using System.ComponentModel.DataAnnotations;

namespace Applaunch.Models.Files;

public class FileSaveDto
{
    public Guid FileId { get; set; }
    [StringLength(255)]
    public string FileName { get; set; }
    [StringLength(75)]
    public string Category { get; set; }
    public Byte[] FileBytes { get; set; }
}