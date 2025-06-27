using System.Security.Claims;
using AppLaunch.Services;
using AppLaunch.Services.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MyIdentityUserAccessor = AppLaunch.Admin.Account.IdentityUserAccessor;

public class IdentityRegistrar
{
    private readonly IServiceProvider _provider;

    public IdentityRegistrar(IServiceProvider provider)
    {
        _provider = provider;
    }

    public static void Register(IServiceCollection services)    {
        if (services is null)
        {
            throw new InvalidOperationException("Unable to access the service collection.");
        }
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 10;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddRoles<IdentityRole>()
            .AddDefaultTokenProviders();

        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<MyIdentityUserAccessor>();
        services.AddScoped<IUserService, UserService>();
        
        // Add RoleManager and UserManager 
        services.AddScoped<IUserStore<ApplicationUser>, UserStore<ApplicationUser, IdentityRole, ApplicationDbContext>>();
        services.AddScoped<IRoleStore<IdentityRole>, RoleStore<IdentityRole, ApplicationDbContext>>();
        services.AddScoped<UserManager<ApplicationUser>>();
        services.AddScoped<RoleManager<IdentityRole>>();
        services.Configure<IdentityOptions>(options => { options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role; });
    }
}