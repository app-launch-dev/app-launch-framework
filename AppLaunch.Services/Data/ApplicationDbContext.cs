using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppLaunch.Services.Data
{

  public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : IdentityDbContext<ApplicationUser>(options)
  {
  
    public virtual DbSet<Site> Sites { get; set; }
    public virtual DbSet<SiteHost> SiteHosts { get; set; }
    public virtual DbSet<File> Files { get; set; }
    
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
       try
       {
           base.OnModelCreating(modelBuilder);
           modelBuilder.HasDefaultSchema("applaunch");
           
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
           
           modelBuilder.Entity<IdentityRole>().HasData(
               new IdentityRole { Id = new Guid("21ab5e5f-43e1-40eb-9bef-b26ac0dc6751").ToString(), Name = "Admin", NormalizedName = "ADMIN" },
               new IdentityRole { Id = new Guid("0b5b6765-40c8-410a-9271-92651118a908").ToString(), Name = "Manager", NormalizedName = "MANAGER" },
               new IdentityRole { Id = new Guid("d68c4c67-c79b-4f43-8795-feddccaf0832").ToString(), Name = "Public", NormalizedName = "PUBLIC" }
           );
        
       }
       catch (Exception ex)
       {
           Console.WriteLine(ex);
       }
  
   }
  }
}
