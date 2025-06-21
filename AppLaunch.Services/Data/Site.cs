using System.ComponentModel.DataAnnotations;

namespace AppLaunch.Services.Data;

public class Site
{
    [Key]
    public Guid SiteId { get; set; }
    public Guid ThemeId { get; set; }
    [StringLength(150)]
    public string SiteName { get; set; }
    [StringLength(150)]
    public string DefaultEmailFrom { get; set; }
    [StringLength(150)]
    public string Layout { get; set; }
    public string OverrideCSS { get; set; }
    public string? GlobalContent{ get; set; }
    public string ContentKey { get; set; }
    public bool AllowSignUp { get; set; }
    public string? LoginOverrideCSS { get; set; }
    public string? LoginLogoUrl { get; set; }
    public string? AwsSesAccessKey { get; set; }
    public string? AwsSesSecretKey { get; set; }
    public string? GitHubToken { get; set; }
    public string? GitHubOwner { get; set; }
    public string? GitHubRepo { get; set; }
    public string? GitHubWorkflowFile { get; set; }
    public string? ColorPrimaryLight { get; set; }
    public string? ColorSecondaryLight { get; set; }
    public string? ColorAppbarBackgroundLight { get; set; }
    public string? ColorAppbarTextLight { get; set; }
    public string? ColorBackgroundLight { get; set; }
    public string? ColorTextPrimaryLight { get; set; }
    public string? ColorTextSecondaryLight { get; set; }
    public string? ColorDrawerBackgroundLight { get; set; }
    public string? ColorDrawerTextLight { get; set; }
    public string? ColorSurfaceLight { get; set; }
    public string? ColorPrimaryDark { get; set; }
    public string? ColorSecondaryDark { get; set; }
    public string? ColorAppbarBackgroundDark { get; set; }
    public string? ColorAppbarTextDark { get; set; }
    public string? ColorBackgroundDark { get; set; }
    public string? ColorTextPrimaryDark { get; set; }
    public string? ColorTextSecondaryDark { get; set; }
    public string? ColorDrawerBackgroundDark { get; set; }
    public string? ColorDrawerTextDark { get; set; }
    public string? ColorSurfaceDark { get; set; }
}