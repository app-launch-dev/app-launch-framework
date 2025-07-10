using System.Reflection;
using AppLaunch.Core.Components;
using AppLaunch.Services;
using AppLaunch.Services.Data;
using Microsoft.AspNetCore.Http.Features;
using MudBlazor;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using MyIdentityRedirectManager = AppLaunch.Admin.Account.IdentityRedirectManager;
using MyIdentityRevalidatingAuthenticationStateProvider =
    AppLaunch.Admin.Account.IdentityRevalidatingAuthenticationStateProvider;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("applaunch.json", optional: true, reloadOnChange: true);
builder.Services.AddSingleton<IConnectionStringProvider, ConnectionStringProvider>();

builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
    {
        var connectionProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
        var connectionString = connectionProvider.GetConnectionString();
        
        options.ConfigureWarnings(warnings => 
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        options.UseSqlServer(
            connectionString,
            sqlOptions => sqlOptions.MigrationsAssembly("AppLaunch.Services")
        );
    },
    contextLifetime: ServiceLifetime.Scoped,
    optionsLifetime: ServiceLifetime.Singleton
);

//Configure AddDbContextFactory to use the same provider
builder.Services.AddDbContextFactory<ApplicationDbContext>((serviceProvider, options) =>
    {
        var connectionProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
        var connectionString = connectionProvider.GetConnectionString();
        
        options.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        options.UseSqlServer(
            connectionString,
            sqlOptions => sqlOptions.MigrationsAssembly("AppLaunch.Services")
        );
    }
);

builder.Services.AddSingleton<IdentityRegistrar>();
builder.Services.AddSingleton<StartupHelper>();
builder.Services.AddScoped<MyIdentityRedirectManager>(); //todo: may not be used
builder.Services.AddScoped<AuthenticationStateProvider, MyIdentityRevalidatingAuthenticationStateProvider>();
IdentityRegistrar.Register(builder.Services);
StartupHelper.Register(builder.Services);

//Max form size
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1 * 1000 * 1000 * 1000; // 1 GB
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1 * 1000 * 1000 * 1000; // 1 GB
});

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
});

var existingAssemblies = new List<Assembly> { typeof(AppLaunch.Admin._Imports).Assembly };
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseMiddleware<TenantMiddleware>();
app.UseMiddleware<CookieMiddleware>();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.MapControllers();
app.MapRazorPages();

app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(existingAssemblies.ToArray()) // Always add Admin
    .AddInteractiveServerRenderMode();

app.UseAuthorization();
app.MapAdditionalIdentityEndpoints();

app.Run();