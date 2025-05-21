using Microsoft.AspNetCore.Http;
namespace AppLaunch.Services
{
  public class CoreXCookieMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CoreXCookieMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
    {
      _next = next;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        string corexId;

        // Try to get the unique ID from the cookie
        if (context == null) throw new Exception("Could not access HttpContext during CoreXId Cookie check");
       
        if (context.Request.Cookies.TryGetValue("CoreXId", out corexId))
        {
          context.Items["CoreXId"] = corexId;
        }
        else
        {
            // Generate a new unique ID
            corexId = Guid.NewGuid().ToString();
            System.Diagnostics.Debug.WriteLine($"Creating new CoreXId: {corexId}");
            
            // Set the persistent cookie
            context.Response.Cookies.Append("CoreXId", corexId, new CookieOptions
            {
              HttpOnly = true,
              Secure = true, // Set to true if using HTTPS
              IsEssential = true,
              Expires = DateTimeOffset.UtcNow.AddYears(1) // Set long expiration
            });
       
          context.Items["CoreXId"] = corexId;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        throw;
      }
      await _next(context);
    }
  }
}