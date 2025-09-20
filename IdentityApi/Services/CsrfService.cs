using System.Security.Cryptography;
using Core;
using Core.Services;

namespace IdentityApi.Services;

public interface ICsrfService
{
    public string GenerateToken();
    public CookieOptions GetCookieOptions();
    public void AppendTokenCookie(HttpContext httpContext);
}
[AppService<ICsrfService>]
public class CsrfService(IWebHostEnvironment environment) : ICsrfService
{
    public string GenerateToken()
    {
        using var rng = RandomNumberGenerator.Create();
        
        byte[] token = new byte[32];
        rng.GetBytes(token);
        
        return Convert.ToBase64String(token);
    }
    public CookieOptions GetCookieOptions() => new()
    {
        HttpOnly    = false,
        Secure      = environment.IsProduction(),
        SameSite    = SameSiteMode.Strict,
        IsEssential = true,
        Expires     = null
    };
    public void AppendTokenCookie(HttpContext httpContext) 
        => httpContext.Response.Cookies.Append(Constants.Tokens.CSRF_TOKEN_COOKIE_NAME, GenerateToken(), GetCookieOptions());
}