using Microsoft.AspNetCore.Http;
namespace AppLaunch.Services
{
  public class CookieMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
    {
      _next = next;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        string CoreId;

        // Try to get the unique ID from the cookie
        if (context == null) throw new Exception("Could not access HttpContext during CoreId Cookie check");
       
        if (context.Request.Cookies.TryGetValue("CoreId", out CoreId))
        {
          context.Items["CoreId"] = CoreId;
        }
        else
        {
            // Generate a new unique ID
            CoreId = Guid.NewGuid().ToString();
            System.Diagnostics.Debug.WriteLine($"Creating new CoreId: {CoreId}");
            
            // Set the persistent cookie
            context.Response.Cookies.Append("CoreId", CoreId, new CookieOptions
            {
              HttpOnly = true,
              Secure = true, // Set to true if using HTTPS
              IsEssential = true,
              Expires = DateTimeOffset.UtcNow.AddYears(1) // Set long expiration
            });
       
          context.Items["CoreId"] = CoreId;
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