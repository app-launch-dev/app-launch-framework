using AppLaunch.Models;
using AppLaunch.Models.Settings;
using AppLaunch.Services.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AppLaunch.Services;

public interface ISettingsService
{
    Task<CoreResponse<SettingsModel>> GetSettings();
    Task<CoreResponse> SaveSettings(SettingsModel model);
    event Action? SettingsChanged;
}

public class SettingsService(IDbContextFactory<ApplicationDbContext> contextFactory) : ISettingsService
{
    public event Action? SettingsChanged;
    
    public async Task<CoreResponse<SettingsModel>> GetSettings()
    {
        CoreResponse<SettingsModel> myResponse = new();
        try
        {
            using var context = contextFactory.CreateDbContext();
            var response = await (from s in context.Sites
                select s).FirstOrDefaultAsync();

            SettingsModel mySettings = new()
            {
                DefaultEmailFrom = response.DefaultEmailFrom,
                SettingId = response.SiteId,
                SiteName = response.SiteName,
                ThemeId = response.ThemeId,
                OverrideCSS = response.OverrideCSS,
                Layout = response.Layout,
                ContentKey = response.ContentKey,
                AllowSignUp = response.AllowSignUp,
                LoginLogoUrl = response.LoginLogoUrl,
                LogoUrl = response.LogoUrl,
                LoginOverrideCSS = response.LoginOverrideCSS,
                AwsSesAccessKey = response.AwsSesAccessKey,
                AwsSesSecretKey = response.AwsSesSecretKey,
                GitHubWorkflowFile = response.GitHubWorkflowFile,
                GitHubOwner = response.GitHubOwner,
                GitHubRepo = response.GitHubRepo,
                GitHubToken = response.GitHubToken,
                ColorPrimaryLight = response.ColorPrimaryLight,
                ColorSecondaryLight = response.ColorSecondaryLight,
                ColorAppbarBackgroundLight = response.ColorAppbarBackgroundLight,
                ColorAppbarTextLight = response.ColorAppbarTextLight,
                ColorBackgroundLight = response.ColorBackgroundLight,
                ColorTextPrimaryLight = response.ColorTextPrimaryLight,
                ColorTextSecondaryLight = response.ColorTextSecondaryLight,
                ColorDrawerBackgroundLight = response.ColorDrawerBackgroundLight,
                ColorDrawerTextLight = response.ColorDrawerTextLight,
                ColorSurfaceLight = response.ColorSurfaceLight,
                ColorPrimaryDark = response.ColorPrimaryDark,
                ColorSecondaryDark = response.ColorSecondaryDark,
                ColorAppbarBackgroundDark = response.ColorAppbarBackgroundDark,
                ColorAppbarTextDark = response.ColorAppbarTextDark,
                ColorBackgroundDark = response.ColorBackgroundDark,
                ColorTextPrimaryDark = response.ColorTextPrimaryDark,
                ColorTextSecondaryDark = response.ColorTextSecondaryDark,
                ColorDrawerBackgroundDark = response.ColorDrawerBackgroundDark,
                ColorDrawerTextDark = response.ColorDrawerTextDark,
                ColorSurfaceDark = response.ColorSurfaceDark
            };

            myResponse.Data = mySettings;
            myResponse.IsSuccess = true;
        }
        catch (Exception ex)
        {
            myResponse.IsSuccess = false;
            myResponse.Message = ex.Message;
        }

        return myResponse;
    }


    public async Task<CoreResponse> SaveSettings(SettingsModel model)
    {
        CoreResponse myResponse = new();
        try
        {
            using var context = contextFactory.CreateDbContext();
            var existingSettings = await context.Sites.FirstOrDefaultAsync();

            if (existingSettings == null) throw new Exception("Settings not initialized");

            existingSettings.DefaultEmailFrom = model.DefaultEmailFrom;
            existingSettings.ThemeId = model.ThemeId;
            existingSettings.SiteName = model.SiteName;
            existingSettings.Layout = model.Layout;
            existingSettings.OverrideCSS = model.OverrideCSS;
            existingSettings.ContentKey = model.ContentKey;
            existingSettings.AllowSignUp = model.AllowSignUp;
            existingSettings.LogoUrl =model.LogoUrl;
            existingSettings.LoginLogoUrl = model.LoginLogoUrl;
            existingSettings.LoginOverrideCSS = model.LoginOverrideCSS;
            existingSettings.AwsSesAccessKey = model.AwsSesAccessKey;
            existingSettings.AwsSesSecretKey = model.AwsSesSecretKey;
            existingSettings.GitHubWorkflowFile = model.GitHubWorkflowFile;
            existingSettings.GitHubOwner = model.GitHubOwner;
            existingSettings.GitHubRepo = model.GitHubRepo;
            existingSettings.GitHubToken = model.GitHubToken;
            existingSettings.ColorPrimaryLight = model.ColorPrimaryLight;
            existingSettings.ColorSecondaryLight = model.ColorSecondaryLight;
            existingSettings.ColorAppbarBackgroundLight = model.ColorAppbarBackgroundLight;
            existingSettings.ColorAppbarTextLight = model.ColorAppbarTextLight;
            existingSettings.ColorBackgroundLight = model.ColorBackgroundLight;
            existingSettings.ColorTextPrimaryLight = model.ColorTextPrimaryLight;
            existingSettings.ColorTextSecondaryLight = model.ColorTextSecondaryLight;
            existingSettings.ColorDrawerBackgroundLight = model.ColorDrawerBackgroundLight;
            existingSettings.ColorDrawerTextLight = model.ColorDrawerTextLight;
            existingSettings.ColorSurfaceLight = model.ColorSurfaceLight;
            existingSettings.ColorPrimaryDark = model.ColorPrimaryDark;
            existingSettings.ColorSecondaryDark = model.ColorSecondaryDark;
            existingSettings.ColorAppbarBackgroundDark = model.ColorAppbarBackgroundDark;
            existingSettings.ColorAppbarTextDark = model.ColorAppbarTextDark;
            existingSettings.ColorBackgroundDark = model.ColorBackgroundDark;
            existingSettings.ColorTextPrimaryDark = model.ColorTextPrimaryDark;
            existingSettings.ColorTextSecondaryDark = model.ColorTextSecondaryDark;
            existingSettings.ColorDrawerBackgroundDark = model.ColorDrawerBackgroundDark;
            existingSettings.ColorDrawerTextDark = model.ColorDrawerTextDark;
            existingSettings.ColorSurfaceDark = model.ColorSurfaceDark;
            await context.SaveChangesAsync();
            SettingsChanged?.Invoke();
            myResponse.IsSuccess = true;
        }
        catch (Exception ex)
        {
            myResponse.IsSuccess = false;
            myResponse.Message = ex.Message;
        }

        return myResponse;
    }
}