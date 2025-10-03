using System.Net;
using Core.Services;

namespace IdentityApi.Services;

internal interface IIPAddressService
{
    public string? GetClientIP(HttpRequest request);
}
[AppService<IIPAddressService>]
internal class IPAddressService(ILogger<IPAddressService> logger) : IIPAddressService
{
    public string? GetClientIP(HttpRequest request)
    {
        var ipSources = new[]
        {
            request.Headers["CF-Connecting-IP"].FirstOrDefault(),
            request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0].Trim(),
            request.Headers["X-Real-IP"].FirstOrDefault(),
            request.Headers["X-Client-IP"].FirstOrDefault(), 
            request.HttpContext.Connection.RemoteIpAddress?.ToString()
        };
        var ip = ipSources.FirstOrDefault(ip => !string.IsNullOrEmpty(ip) && IsValidIP(ip));
        if (ip != null)
        {
            return ip;
        }
        var fallbackIp = request.HttpContext.Connection.RemoteIpAddress?.ToString();
        logger.LogWarning("Could not determine client IP address, fallback: {IP}", fallbackIp);
        return fallbackIp;
    }
    private static bool IsValidIP(string ip)
    {
        return IPAddress.TryParse(ip, out var address) && 
               !IPAddress.IsLoopback(address)          && 
               !address.Equals(IPAddress.Any) && 
               !address.Equals(IPAddress.IPv6Any);
    }
}