using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppLaunch.Services;

public interface IPlugin
{
    string Name { get; }
    string Description { get; }
    string DbSchema { get; }
    int MajorVersion { get; }
    int MinorVersion { get; }
    int Patch { get; }
    void RegisterServices(IServiceCollection services, IConfiguration config);
    void LoadPlugin();
}
