using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

public interface IHostingService
{
    void RestartApplication();
}

public class HostingService : IHostingService
{
    private readonly IHostApplicationLifetime _appLifetime;

    public HostingService(IHostApplicationLifetime appLifetime)
    {
        _appLifetime = appLifetime;
    }

    public void RestartApplication()
    {
        _appLifetime.StopApplication();
    }
}