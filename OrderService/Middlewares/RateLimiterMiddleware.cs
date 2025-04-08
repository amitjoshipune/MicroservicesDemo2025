using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class RateLimiterMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly Dictionary<string, DateTime> _rateLimiters = new();

    public RateLimiterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString();
        if (clientIp != null)
        {
            if (_rateLimiters.ContainsKey(clientIp))
            {
                var lastRequestTime = _rateLimiters[clientIp];
                if (DateTime.UtcNow - lastRequestTime < TimeSpan.FromMilliseconds(10))
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    return;
                }
            }

            _rateLimiters[clientIp] = DateTime.UtcNow;
        }

        await _next(context);
    }
}
