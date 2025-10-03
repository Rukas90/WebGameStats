using Core;
using FastEndpoints;

namespace IdentityApi.Endpoints;

internal class LogoutEndpoint(IRefreshTokenService refreshTokenService)
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Post("/v1/auth/logout");
        EnableAntiforgery();
    }
    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        await refreshTokenService.RevokeUserTokensAsync(User);
        
        HttpContext.Response.Cookies.Delete(Constants.Tokens.ACCESS_TOKEN_COOKIE_NAME);
        HttpContext.Response.Cookies.Delete(Constants.Tokens.REFRESH_TOKEN_COOKIE_NAME);
        HttpContext.Response.Cookies.Delete(Constants.Tokens.CSRF_TOKEN_COOKIE_NAME);
        
        return TypedResults.Ok(new
        {
            message     = "Logout successful", 
            redirectUrl = "/login"
        });
    }
}