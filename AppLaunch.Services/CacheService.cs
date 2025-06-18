using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace AppLaunch.Services;

public interface ICacheService
{
    Task SaveToCache(string cacheKey, string cacheValue, TimeSpan expiry);
    Task<string> GetFromCache(string cacheKey);
}

public class CacheService(IMemoryCache MemoryCache, IHttpContextAccessor HttpContextAccessor) : ICacheService
{
    public async Task SaveToCache(string cacheKey, string cacheValue, TimeSpan expiry)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        };
        string coreId=await GetCoreId();
        string uniqueCacheKey = $"{coreId}_{cacheKey}";
        MemoryCache.Set(uniqueCacheKey, cacheValue, cacheEntryOptions);
    }

    public async Task<string> GetFromCache(string cacheKey)
    {
        string cacheValue = "";
        try
        {
            string coreId=await GetCoreId();
            if (string.IsNullOrEmpty(coreId)) throw new Exception("");
            string uniqueCacheKey = $"{coreId}_{cacheKey}";
            cacheValue= MemoryCache.Get<string>(uniqueCacheKey);
        }
        catch (Exception ex)
        {
            cacheValue = "";
        }
        return cacheValue;
    }

    private async Task<string> GetCoreId()
    {
        string cookieValue = "";
        try
        {
            cookieValue = HttpContextAccessor.HttpContext.Request.Cookies["CoreId"];
        }
        catch (Exception ex)
        {
            cookieValue = "";
        }

        return cookieValue;
    }
}