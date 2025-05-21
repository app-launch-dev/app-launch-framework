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
            GetSettings:
            var response = await (from s in dbContext.Sites
                select s).FirstOrDefaultAsync();
            if (response == null)
            {
                //initialize settings
                Site site = new()
                {
                    DefaultEmailFrom = "noreply@domain.com",
                    SiteId = Guid.NewGuid(),
                    SiteName = "CoreX Sample Site",
                    ThemeId = new Guid("d145c1f7-2193-473a-aa6f-3ceed8343a44"),
                    Layout = "PortoLayout.razor",
                    OverrideCSS = "",
                    GlobalContent = null,
                    ContentKey = Guid.NewGuid().ToString(),
                    AllowSignUp = false,
                    LoginLogoUrl = null,
                    LoginOverrideCSS = null
                };
                await dbContext.Sites.AddAsync(site);
                await dbContext.SaveChangesAsync();
                goto GetSettings;
            }

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
                LoginOverrideCSS = response.LoginOverrideCSS
            };

            // if (response.GlobalContent != null)
            //     mySettings.GlobalContent =
            //         JsonConvert.DeserializeObject<List<ThemeComponentModel>>(response.GlobalContent);

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

            // if (model.GlobalContent != null)
            //     existingSettings.GlobalContent = JsonConvert.SerializeObject(model.GlobalContent);


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