using Core.Config.Cookies;
using Core;
using IdentityApi.Responses;

namespace IdentityApi;

internal static class TokenExtensions
{
    public static HttpContext SetToHttpResponse(this TokensPairResponse tokens, HttpContext context, bool isProduction)
    {
        context.Response.Cookies.Append(
            Constants.Tokens.ACCESS_TOKEN_COOKIE_NAME, tokens.AccessToken,
            CookiesConfiguration.GetSecureCookieOptionsHttpOnly(Constants.Tokens.ACCESS_TOKEN_EXPIRY_TIME, isProduction));

        context.Response.Cookies.Append(
            Constants.Tokens.REFRESH_TOKEN_COOKIE_NAME, tokens.RefreshToken,
            CookiesConfiguration.GetSecureCookieOptionsHttpOnly(Constants.Tokens.REFRESH_TOKEN_EXPIRY_TIME, isProduction));

        return context;
    }
}