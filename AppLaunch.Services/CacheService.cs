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
        string coreXId=await GetCoreXId();
        string uniqueCacheKey = $"{coreXId}_{cacheKey}";
        MemoryCache.Set(uniqueCacheKey, cacheValue, cacheEntryOptions);
    }

    public async Task<string> GetFromCache(string cacheKey)
    {
        string cacheValue = "";
        try
        {
            string coreXId=await GetCoreXId();
            if (string.IsNullOrEmpty(coreXId)) throw new Exception("");
            string uniqueCacheKey = $"{coreXId}_{cacheKey}";
            cacheValue= MemoryCache.Get<string>(uniqueCacheKey);
        }
        catch (Exception ex)
        {
            cacheValue = "";
        }
        return cacheValue;
    }

    private async Task<string> GetCoreXId()
    {
        string cookieValue = "";
        try
        {
            cookieValue = HttpContextAccessor.HttpContext.Request.Cookies["CoreXId"];
        }
        catch (Exception ex)
        {
            cookieValue = "";
        }

        return cookieValue;
    }
}