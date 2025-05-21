
namespace AppLaunch.Services;

public interface IAlertService
{
    event Func<Task>? OnAlertTriggered;
    Task ClearAlerts();
    Task<Dictionary<string,string>> GetAlerts();

    enum AlertType
    {
        Error,
        Success,
        Info,
        Warning
    }

    Task AddAlert(string alertMessage, IAlertService.AlertType alertType, bool showAlert = true);
}

public class AlertService (ICacheService CacheService) : IAlertService
{
    // Define the event
    public event Func<Task>? OnAlertTriggered;
    
    public async Task AddAlert(string alertMessage, IAlertService.AlertType alertType, bool showAlert = true)
    {
        await CacheService.SaveToCache("AlertMessage", alertMessage,TimeSpan.FromMinutes(5));
        
        switch (alertType)
        {
            case IAlertService.AlertType.Info:
                await CacheService.SaveToCache("AlertType", "alert alert-info",TimeSpan.FromMinutes(5));
                break;
            case IAlertService.AlertType.Error:
                await CacheService.SaveToCache("AlertType", "alert alert-danger",TimeSpan.FromMinutes(5));
                break;
            case IAlertService.AlertType.Warning:
                await CacheService.SaveToCache("AlertType", "alert alert-warning",TimeSpan.FromMinutes(5));
                break;
            case IAlertService.AlertType.Success:
                await CacheService.SaveToCache("AlertType", "alert alert-success",TimeSpan.FromMinutes(5));
                break;
            default:
                await CacheService.SaveToCache("AlertType", "alert alert-info",TimeSpan.FromMinutes(5));
                break;
        }

        if (showAlert)
        {
            if (OnAlertTriggered != null)
            {
                await OnAlertTriggered.Invoke();
            }
        }
    }

    public async Task ClearAlerts()
    {
        await CacheService.SaveToCache("AlertType", "", TimeSpan.FromMicroseconds(1));
        await CacheService.SaveToCache("AlertMessage", "", TimeSpan.FromMicroseconds(1));
    }

    public async Task<Dictionary<string,string>> GetAlerts()
    {
        Dictionary<string, string> AlertData = new();
        try
        {
            string AlertMessage = await CacheService.GetFromCache("AlertMessage");
            string AlertType = await CacheService.GetFromCache("AlertType");
            
            if (!string.IsNullOrEmpty(AlertMessage) && !string.IsNullOrEmpty(AlertType))
            {
                AlertData.Add(AlertMessage,AlertType);
            }
        }
        catch (Exception ex)
        {
            AlertData = new();
        }
        return AlertData;
    }
}