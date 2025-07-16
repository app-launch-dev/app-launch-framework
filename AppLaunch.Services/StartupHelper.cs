using System.Reflection;
using AppLaunch.Services.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AppLaunch.Services;

public class StartupHelper
{
    private readonly IServiceProvider _provider;

    public StartupHelper(IServiceProvider provider)
    {
        _provider = provider;
    }

    public static void Register(IServiceCollection services, List<Assembly> pluginAssemblies, IConfiguration configuration)
    {
        if (services is null)
        {
            throw new InvalidOperationException("Unable to access the service collection.");
        }
        
        //register plugin dbcontexts
        foreach (var plugin in pluginAssemblies)
        {
            RegisterPluginDbContexts(services,plugin,configuration); 
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
        services.AddScoped<IUpdateService, UpdateService>();
        services.AddScoped<IPluginMenuProvider, PluginMenuProvider>();
        
    }
    
    public static void RegisterPluginDbContexts(IServiceCollection services, Assembly pluginAssembly, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var dbContextTypes = pluginAssembly.GetTypes()
            .Where(t =>
                typeof(DbContext).IsAssignableFrom(t) &&
                !t.IsAbstract &&
                t.IsPublic &&
                t.Namespace != null &&
                t.Namespace.StartsWith("AppLaunch.Plugins."))
            .ToList();


        foreach (var dbContextType in dbContextTypes)
        {
            var method = typeof(EntityFrameworkServiceCollectionExtensions)
                .GetMethods()
                .First(m => m.Name == "AddDbContext" &&
                            m.IsGenericMethodDefinition &&
                            m.GetParameters().Any(p => p.ParameterType == typeof(Action<DbContextOptionsBuilder>)));

            var generic = method.MakeGenericMethod(dbContextType);

            generic.Invoke(null, new object[]
            {
                services,
                (Action<DbContextOptionsBuilder>)(options =>
                    options.UseSqlServer(connectionString))
            });

            Console.WriteLine($"✅ Registered {dbContextType.Name} with default connection string.");
        }
    }

}