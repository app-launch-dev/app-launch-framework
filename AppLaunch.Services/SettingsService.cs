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
}

public class SettingsService(ApplicationDbContext dbContext) : ISettingsService
{
    public async Task<CoreResponse<SettingsModel>> GetSettings()
    {
        CoreResponse<SettingsModel> myResponse = new();
        try
        {
            var response = await (from s in dbContext.Sites
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
                LoginOverrideCSS = response.LoginOverrideCSS,
                AwsSesAccessKey = response.AwsSesAccessKey,
                AwsSesSecretKey = response.AwsSesSecretKey,
                GitHubWorkflowFile = response.GitHubWorkflowFile,
                GitHubOwner = response.GitHubOwner,
                GitHubRepo = response.GitHubRepo,
                GitHubToken = response.GitHubToken
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
            var existingSettings = await dbContext.Sites.FirstOrDefaultAsync();

            if (existingSettings == null) throw new Exception("Settings not initialized");

            existingSettings.DefaultEmailFrom = model.DefaultEmailFrom;
            existingSettings.ThemeId = model.ThemeId;
            existingSettings.SiteName = model.SiteName;
            existingSettings.Layout = model.Layout;
            existingSettings.OverrideCSS = model.OverrideCSS;
            existingSettings.ContentKey = model.ContentKey;
            existingSettings.AllowSignUp = model.AllowSignUp;
            existingSettings.LoginLogoUrl = model.LoginLogoUrl;
            existingSettings.LoginOverrideCSS = model.LoginOverrideCSS;
            existingSettings.AwsSesAccessKey = model.AwsSesAccessKey;
            existingSettings.AwsSesSecretKey = model.AwsSesSecretKey;
            existingSettings.GitHubWorkflowFile = model.GitHubWorkflowFile;
            existingSettings.GitHubOwner = model.GitHubOwner;
            existingSettings.GitHubRepo = model.GitHubRepo;
            existingSettings.GitHubToken = model.GitHubToken;
            await dbContext.SaveChangesAsync();
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