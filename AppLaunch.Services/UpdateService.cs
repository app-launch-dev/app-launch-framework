using AppLaunch.Models;
using AppLaunch.Models.Updates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppLaunch.Services;

public interface IUpdateService
{
    Task<CoreResponse<CoreUpdateModel>> GetInstalledVersion();
    Task<CoreResponse<CoreUpdateModel>> GetLatestVersion();
}

public class UpdateService (ISettingsService settingsService) : IUpdateService
{
    public async Task<CoreResponse<CoreUpdateModel>> GetInstalledVersion()
    {
        CoreResponse<CoreUpdateModel> myResponse = new();
        try
        {
            if (!File.Exists("applaunch_lastupdate.json")) throw new Exception("Update file not found");
            var json = await File.ReadAllTextAsync("applaunch_lastupdate.json");
            var updateInfo = JsonConvert.DeserializeObject<CoreUpdateModel>(json);
            myResponse.Data = updateInfo!;
            myResponse.IsSuccess = true;
        }
        catch (Exception ex)
        {
            myResponse.IsSuccess=false;
            myResponse.Message=ex.Message;
        }
        return myResponse;
    }
    
    public async Task<CoreResponse<CoreUpdateModel>> GetLatestVersion()
    {
        CoreResponse<CoreUpdateModel> myResponse = new();
        try
        {
            var settingsResponse = await settingsService.GetSettings();
            if (!settingsResponse.IsSuccess) throw new Exception(settingsResponse.Message);
            var settings = settingsResponse.Data;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(settings.GitHubOwner);

            var response = await client.GetAsync($"https://api.github.com/repos/app-launch-dev/app-launch-template/commits/main");
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var content = JObject.Parse(jsonString);

            var sha = content["sha"]?.ToString();
            var dateString = content["commit"]?["committer"]?["date"]?.ToString();
            var date = DateTimeOffset.TryParse(dateString, out var parsedDate) ? parsedDate : default;

            myResponse.Data = new CoreUpdateModel
            {
                Sha = sha ?? string.Empty,
                Date = date
            };

            myResponse.IsSuccess = true;
        }
        catch (Exception ex)
        {
            myResponse.IsSuccess = false;
            myResponse.Message = $"Unable to check for latest version: {ex.Message}";
        }
        return myResponse;
    }

    
}