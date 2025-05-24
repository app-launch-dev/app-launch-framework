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
       
       
       //Default Theme
       // modelBuilder.Entity<Theme>().HasData(
       //     new Theme()
       //     {
       //         ThemeName = "MudBlazor",
       //         ThemeId = new Guid("d145c1f7-2193-473a-aa6f-3ceed8343a44")
       //     }
       // );
       
    
       //Default Site
         modelBuilder.Entity<Site>().HasData(
             new Site()
             {
                SiteId = new Guid("16c4e35d-1ce6-404d-9a61-c61b55f51a17"),
                DefaultEmailFrom = "no-reply@mydomain.com",
                GlobalContent = null,
                Layout="AppLaunchDefault.razor",
                OverrideCSS = "",
                SiteName = "My AppLaunch Site",
                ThemeId = new Guid("d145c1f7-2193-473a-aa6f-3ceed8343a44"),
                ContentKey = "f15998d5-c4b7-4719-b0b4-c86761ce9ae8",
                AllowSignUp = false
             }
         );
       
  
   }
  }
}
