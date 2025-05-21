using AppLaunch.Services.Data;
using Microsoft.AspNetCore.Http;

namespace AppLaunch.Services
{
    public class CoreXTenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CoreXTenantMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InvokeAsync(HttpContext context, ITenantService tenantService, ApplicationDbContext dbContext)
        {
            try
            {
                var host = context.Request.Host.Host;
                Guid siteId = LookupSiteIdFromHost(host, dbContext);

                // Set the tenant information so it's available for dependency injection
                tenantService.SiteId = siteId;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            await _next(context);
        }

        private Guid LookupSiteIdFromHost(string host, ApplicationDbContext dbContext)
        {
            var tenant = dbContext.SiteHosts.FirstOrDefault(s => s.HostName == host);
            return tenant != null ? tenant.SiteId : new Guid("16c4e35d-1ce6-404d-9a61-c61b55f51a17");
        }
    }
}