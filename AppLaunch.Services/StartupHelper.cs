using AppLaunch.Services.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace AppLaunch.Services;

public class StartupHelper
{
    private readonly IServiceProvider _provider;

    public StartupHelper(IServiceProvider provider)
    {
        _provider = provider;
    }

    public static void Register(IServiceCollection services)
    {
        if (services is null)
        {
            throw new InvalidOperationException("Unable to access the service collection.");
        }

        services.AddRazorComponents()
            .AddInteractiveServerComponents();
        services.AddRazorPages();
        services.AddControllersWithViews();
        services.AddServerSideBlazor();
       

        services.AddCascadingAuthenticationState();
        

        services.AddTransient<IEmailSender, AwsSesEmailService>();
        services.AddTransient<IEmailSender<ApplicationUser>>(provider =>
            new AppLaunch.Services.AwsSesIdentityEmailService(provider.GetRequiredService<ISettingsService>(),
                provider.GetRequiredService<IEmailSender>())
        );

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddSingleton<PluginManager>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IPluginMenuProvider, PluginMenuProvider>();
        
    }
}