# Migrations
Migrations in AppLaunch are generated in the AppLaunch.Services project and are based on the Identity Entity Framework libraries. CLI migration commands
should be executed from the app-launch-framework folder and not in a specific project folder.

<tip> Note: By default, migrations are automatically applied during startup.</tip>

## Prerequisites
1. AppLaunch.Services web app project that references AppLaunch.Core
2. AppLaunch.Core Razor Class library
3. Ensure the dotnet-ef Tool is installed
```
dotnet tool install --global dotnet-ef

```
4. Install Nuget packages inside the AppLaunch.Services project:
```
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

## AppLaunch Migrations
1. Create an ApplicationDbContext in the AppLaunch.Services project Data folder.
```c#
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppLaunch.Services.Data
{

  public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : IdentityDbContext<ApplicationUser>(options)
  {
  
    public virtual DbSet<Site> Sites { get; set; }
    public virtual DbSet<SiteHost> SiteHosts { get; set; }
    
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
       try
       {
           base.OnModelCreating(modelBuilder);
           modelBuilder.HasDefaultSchema("applaunch");
       }
       catch (Exception ex)
       {
           Console.WriteLine(ex);
       }
   }
  }
}

```
2.From the app-launch folder, run the following command:
```
dotnet ef migrations add InitialMigration --project AppLaunch.Services --startup-project AppLaunch.Core
```

3. To manually run the migration, you must specify the context, project and startup project.
```
dotnet ef database update --context ApplicationDbContext --project AppLaunch.Services --startup-project AppLaunch.Core
```

4. Enable automatic migrations with the following section in program.cs. 
<tip>Add the try catch to prevent failure on first startup when a connection string is not present.</tip>
```c#
// Apply migrations at startup
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}
catch (Exception ex)
{
   Console.WriteLine($"Skipping migrations: {ex.Message}");
}
```

## Plugin Migrations
