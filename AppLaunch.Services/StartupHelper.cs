using System.Reflection;
using AppLaunch.Services.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
            RegisterPluginDbContextFactories(services,plugin,configuration); 
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
    
    public static void RegisterPluginDbContextFactories(
        IServiceCollection services,
        Assembly pluginAssembly,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var dbContextTypes = pluginAssembly.GetTypes()
            .Where(t =>
                typeof(DbContext).IsAssignableFrom(t) &&
                !t.IsAbstract &&
                t.IsPublic)
            .ToList();

        foreach (var dbContextType in dbContextTypes)
        {
            // var method = typeof(EntityFrameworkServiceCollectionExtensions)
            //     .GetMethods(BindingFlags.Public | BindingFlags.Static)
            //     .FirstOrDefault(m =>
            //         m.Name == "AddDbContextFactory" &&
            //         m.IsGenericMethodDefinition &&
            //         m.GetParameters().Length >= 2 &&
            //         m.GetParameters()[0].ParameterType == typeof(IServiceCollection));
            
            var method = typeof(EntityFrameworkServiceCollectionExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.Name == "AddDbContextFactory" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 3 &&
                    m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Action<,>));

            
            
            var methods = typeof(EntityFrameworkServiceCollectionExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == "AddDbContextFactory");

            foreach (var m in methods)
            {
                Console.WriteLine($"Method: {m.Name}<{string.Join(", ", m.GetGenericArguments().Select(t => t.Name))}>");
                Console.WriteLine($"  Parameters: {string.Join(", ", m.GetParameters().Select(p => p.ParameterType.FullName))}");
            }

            

            if (method == null)
            {
                Console.WriteLine($"Couldn't find AddDbContextFactory method for {dbContextType.FullName}");
                continue;
            }

            var generic = method.MakeGenericMethod(dbContextType);

            try
            {
                var delegateType = typeof(Action<,>).MakeGenericType(
                    typeof(IServiceProvider),
                    typeof(DbContextOptionsBuilder)
                );

                var optionsDelegate = Delegate.CreateDelegate(
                    delegateType,
                    typeof(StartupHelper).GetMethod(nameof(ConfigureDbContext), BindingFlags.NonPublic | BindingFlags.Static)
                );


                generic.Invoke(null, new object[]
                {
                    services,
                    (Action<IServiceProvider, DbContextOptionsBuilder>)((sp, options) =>
                    {
                        var connectionProvider = sp.GetRequiredService<IConnectionStringProvider>();
                        var connectionString = connectionProvider.GetConnectionString();

                        options.UseSqlServer(connectionString, sql =>
                            sql.MigrationsAssembly("AppLaunch.Services"));
                    }),
                    ServiceLifetime.Scoped
                });


                Console.WriteLine($"🏭 Registered factory for {dbContextType.FullName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to register factory for {dbContextType.FullName}: {ex.Message}");
            }
        }
        
        
    }

    private static void ConfigureDbContext(IServiceProvider sp, DbContextOptionsBuilder options)
    {
        var connectionProvider = sp.GetRequiredService<IConnectionStringProvider>();
        var connectionString = connectionProvider.GetConnectionString();

        options.UseSqlServer(connectionString, sql =>
            sql.MigrationsAssembly("AppLaunch.Services"));
    }


}