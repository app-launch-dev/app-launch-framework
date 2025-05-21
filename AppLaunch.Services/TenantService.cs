namespace AppLaunch.Services;

public interface ITenantService
{
    Guid SiteId { get; set; }
}

public class TenantService : ITenantService
{
    public Guid SiteId { get; set; }
}