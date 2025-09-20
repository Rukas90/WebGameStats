using Microsoft.AspNetCore.Http;

namespace Core.Config.Cookies;

public static class CookiesConfiguration
{
    public static CookieOptions GetSecureCookieOptionsHttpOnly(TimeSpan maxAge, bool isProduction) 
        => new()
        {
            HttpOnly    = true,
            Secure      = isProduction,
            SameSite    = isProduction ? SameSiteMode.Strict : SameSiteMode.Lax,
            Path        = "/",
            MaxAge      = maxAge,
            IsEssential = true
        };
}