using System.Reflection;
using System.Security.Claims;
using AppLaunch.Core.Components;
using AppLaunch.Services.Data;
using AppLaunch.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity;
using MyIdentityRedirectManager = AppLaunch.Admin.Account.IdentityRedirectManager;
using MyIdentityRevalidatingAuthenticationStateProvider =
    AppLaunch.Admin.Account.IdentityRevalidatingAuthenticationStateProvider;
using MyIdentityUserAccessor = AppLaunch.Admin.Account.IdentityUserAccessor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("applaunch.json", optional: true, reloadOnChange: true);
//List<Assembly> additionalAssemblies= new();
//List<Assembly> additionalAssemblies= new();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<MyIdentityUserAccessor>();
builder.Services.AddScoped<MyIdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, MyIdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, AppLaunch.Admin.Account.IdentityNoOpEmailSender>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"],
        b => b.MigrationsAssembly("AppLaunch.Services")));

builder.Services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            // Password Requirements
            //options.Password.RequireDigit = true; // Must contain a digit (0-9)
            //options.Password.RequireLowercase = true; // Must contain a lowercase letter (a-z)
            //options.Password.RequireUppercase = true; // Must contain an uppercase letter (A-Z)
            options.Password.RequireNonAlphanumeric = true; // Must contain a symbol (!, @, #, etc.)
            options.Password.RequiredLength = 10; // Minimum password length
            //options.Password.RequiredUniqueChars = 3; // Require at least 3 unique characters
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

// Register HttpClient
builder.Services.AddHttpClient();
//builder.Services.AddSingleton<IEmailSender<ApplicationUser>, AppLaunch.Core.Components.Account.IdentityNoOpEmailSender>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
// builder.Services.AddScoped<IFormHandlerService, FormHandlerService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
// builder.Services.AddScoped<ICoreXThemeService, CoreXThemeService>();
// builder.Services.AddScoped<IPageService, PageService>();
// builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAlertService, AlertService>();

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

static Assembly[] LoadPluginAssemblies()
{
    var pluginPath = Path.Combine(AppContext.BaseDirectory, "Plugins");
    return Directory.EnumerateFiles(pluginPath, "*.dll")
        .Select(Assembly.LoadFrom)
        .ToArray();
}

var additionalAssemblies = LoadPluginAssemblies().ToList();

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
// try
// {
//     using var scope = app.Services.CreateScope();
//     var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     dbContext.Database.Migrate();
// }
// catch (Exception ex)
// {
//    Console.WriteLine($"Skipping migrations: {ex.Message}");
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
    .AddAdditionalAssemblies(typeof(AppLaunch.Admin._Imports).Assembly)
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