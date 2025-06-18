using System.ComponentModel.DataAnnotations;

namespace AppLaunch.Services.Data;

public class File
{
    [Key]
    public Guid FileId { get; set; }
    public Guid SiteId { get; set; }
    [StringLength(255)]
    public string FileName { get; set; }
    public byte[] FileBytes { get; set; }
    [StringLength(75)]
    public string Category { get; set; }
    [StringLength(50)]
    public string MimeType { get; set; }
}