using System.ComponentModel.DataAnnotations;

namespace AppLaunch.Models.Settings;

public class SettingsModel
{
    public Guid SettingId { get; set; }
    public Guid ThemeId { get; set; }
    [StringLength(150)]
    public string SiteName { get; set; }
    [StringLength(150)]
    public string DefaultEmailFrom { get; set; }
    [StringLength(1000)]
    public string LogoUrl { get; set; }
    [StringLength(150)]
    public string Layout { get; set; }
    public string OverrideCSS { get; set; }
    public string ContentKey { get; set; }
    public bool AllowSignUp { get; set; }
    public string? LoginOverrideCSS { get; set; }
    public string? LoginLogoUrl { get; set; }
    public string? AwsSesAccessKey { get; set; }
    public string? AwsSesSecretKey { get; set; }
}

