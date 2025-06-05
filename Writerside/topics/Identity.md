# Identity

Create the following lines in program.cs:


```c#
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<MyIdentityUserAccessor>();
builder.Services.AddScoped<MyIdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, MyIdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();
```