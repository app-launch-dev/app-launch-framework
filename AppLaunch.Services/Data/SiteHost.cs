using System.ComponentModel.DataAnnotations;

namespace AppLaunch.Services.Data;

public class SiteHost
{
    [Key]
    public Guid SiteHostId { get; set; }
    public Guid SiteId { get; set; }
    [StringLength(255)]
    public string HostName { get; set; }
}