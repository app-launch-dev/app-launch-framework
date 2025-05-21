using System.Reflection;
using AppLaunch.Components;
using AppLaunch.Services.Data;
using AppLaunch.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using IdentityRedirectManager = AppLaunch.Components.Account.IdentityRedirectManager;
using IdentityRevalidatingAuthenticationStateProvider = AppLaunch.Components.Account.IdentityRevalidatingAuthenticationStateProvider;
using IdentityUserAccessor = AppLaunch.Components.Account.IdentityUserAccessor;
using MudBlazor.Services;
var builder = WebApplication.CreateBuilder(args);
//List<Assembly> additionalAssemblies= new();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"], 
        b => b.MigrationsAssembly("AppLaunch")));

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
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

// Register HttpClient
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, AppLaunch.Components.Account.IdentityNoOpEmailSender>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddSingleton<IHostingService, HostingService>();
// builder.Services.AddScoped<IFormHandlerService, FormHandlerService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
// builder.Services.AddScoped<ICoreXThemeService, CoreXThemeService>();
// builder.Services.AddScoped<IPageService, PageService>();
// builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IAlertService, AlertService>();

var app = builder.Build();

static Assembly[] LoadPluginAssemblies()
{
    var pluginPath = Path.Combine(AppContext.BaseDirectory, "Plugins");
    return Directory.EnumerateFiles(pluginPath, "*.dll")
        .Select(Assembly.LoadFrom)
        .ToArray();
}
var additionalAssemblies= LoadPluginAssemblies().ToList();
void ProcessAssembly(Assembly assembly)
{
    try
    {
        Type interfaceType = typeof(IAppLaunchPlugin);
        IEnumerable<Type> pluginTypes = assembly.GetTypes()
            .Where(t => interfaceType.IsAssignableFrom(t) && !t.IsAbstract && t.IsClass);

        foreach (Type type in pluginTypes)
        {
            IAppLaunchPlugin plugin = (IAppLaunchPlugin)Activator.CreateInstance(type);
            plugin.LoadPlugin();
        }
    }
    catch (ReflectionTypeLoadException ex)
    {
        Console.WriteLine($"Error loading types from {assembly.FullName}: {ex.LoaderExceptions.First().Message}");
    }
}

// // Apply migrations at startup
// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     dbContext.Database.Migrate();
// }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
//app.UseMiddleware<CoreXPluginMiddleware>();
app.UseMiddleware<CoreXTenantMiddleware>();
app.UseMiddleware<CoreXCookieMiddleware>();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.MapControllers();
app.MapRazorPages();
app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(AppLaunch.Admin.Startup).Assembly)
    //.AddAdditionalAssemblies(additionalAssemblies.ToArray())
    .AddInteractiveServerRenderMode();
app.UseAuthorization();
// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
{
    ProcessAssembly(assembly);
}
app.Run();