using Core.Config.Cookies;
using Core;
using IdentityApi.Responses;

namespace IdentityApi;

internal static class TokenExtensions
{
    public static HttpResponse SetToHttpResponse(this TokensPairResponse tokens, HttpResponse response, bool isProduction)
    {
        response.Cookies.Append(
            Constants.Tokens.ACCESS_TOKEN_COOKIE_NAME, tokens.AccessToken,
            CookiesConfiguration.GetSecureCookieOptions(Constants.Tokens.ACCESS_TOKEN_EXPIRY_TIME, isProduction));

        response.Cookies.Append(
            Constants.Tokens.REFRESH_TOKEN_COOKIE_NAME, tokens.RefreshToken,
            CookiesConfiguration.GetSecureCookieOptions(Constants.Tokens.REFRESH_TOKEN_EXPIRY_TIME, isProduction));

        return response;
    }
}