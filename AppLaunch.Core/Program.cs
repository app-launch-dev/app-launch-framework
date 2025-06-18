using System.Reflection;
using System.Security.Claims;
using AppLaunch.Core.Components;
using AppLaunch.Services.Data;
using AppLaunch.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MudBlazor;
using MyIdentityRedirectManager = AppLaunch.Admin.Account.IdentityRedirectManager;
using MyIdentityRevalidatingAuthenticationStateProvider =
    AppLaunch.Admin.Account.IdentityRevalidatingAuthenticationStateProvider;
using MyIdentityUserAccessor = AppLaunch.Admin.Account.IdentityUserAccessor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("applaunch.json", optional: true, reloadOnChange: true);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<MyIdentityUserAccessor>();
builder.Services.AddScoped<MyIdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, MyIdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddTransient<IEmailSender, AwsSesEmailService>();
builder.Services.AddTransient<IEmailSender<ApplicationUser>>(provider =>
    new AppLaunch.Services.AwsSesIdentityEmailService(provider.GetRequiredService<ISettingsService>(),provider.GetRequiredService<IEmailSender>())
);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"], b => b.MigrationsAssembly("AppLaunch.Services"));
});

builder.Services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            // Password Requirements
            options.Password.RequireNonAlphanumeric = true; // Must contain a symbol (!, @, #, etc.)
            options.Password.RequiredLength = 10; // Minimum password length
        }
    )
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddRoles<IdentityRole>() // Critical for role management
    .AddDefaultTokenProviders();

//Max form size
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1 * 1000 * 1000 * 1000; // 1 GB
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1 * 1000 * 1000 * 1000; // 1 GB
});

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<PluginManager>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();

// Add RoleManager and UserManager 
builder.Services
    .AddScoped<IUserStore<ApplicationUser>, UserStore<ApplicationUser, IdentityRole, ApplicationDbContext>>();
builder.Services.AddScoped<IRoleStore<IdentityRole>, RoleStore<IdentityRole, ApplicationDbContext>>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
});
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

// Auto-load saved plugins on application startup
var pluginManager = app.Services.GetRequiredService<PluginManager>();
pluginManager.InitializePlugins();

// Get plugin assemblies for routing
var existingAssemblies = new List<Assembly> { typeof(AppLaunch.Admin._Imports).Assembly };
var runningPluginAssemblies = pluginManager.GetRunningPluginAssemblies(existingAssemblies);

// Register Razor components and dynamically add plugin assemblies
app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(existingAssemblies.ToArray()) // Always add Admin
    .AddAdditionalAssemblies(runningPluginAssemblies.ToArray()) // Add dynamically loaded plugins
    .AddInteractiveServerRenderMode();

app.UseAuthorization();
app.MapAdditionalIdentityEndpoints();

app.Run();