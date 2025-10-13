using Core;
using Core.Results;
using FastEndpoints;
using IdentityApi.Models;
using IdentityApi.Responses;

namespace IdentityApi.Endpoints;

internal class RefreshEndpoint(
    IRefreshTokenService refreshTokenService, 
    IJwtService          jwtService,
    IWebHostEnvironment  environment)
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Post(routePatterns: "/v1/auth/session/refresh");
        AllowAnonymous();
        EnableAntiforgery();
        Throttle(hitLimit: 60, durationSeconds: 240);
    }
    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var token = HttpContext.Request.Cookies[Constants.Tokens.REFRESH_TOKEN_COOKIE_NAME];

        if (token is null)
        {
            return TypedResults.Ok("No refresh token found.");
        }
        var refreshToken = await refreshTokenService.GetTokenAsync(token);
        var validation = await ValidateRefreshToken(refreshToken);
        
        if (validation.IsFailure)
        {
            return TypedResults.Problem(validation.Problem);
        }
        await GenerateNewTokens(refreshToken!.User);
        
        return TypedResults.Ok(new
        {
            message = "Refresh successful", 
        });
    }
    private async Task GenerateNewTokens(User user)
    {
        var result = new TokensPairResponse(
            AccessToken:  await jwtService.Generate(user),
            RefreshToken: await refreshTokenService.CreateAsync(user));
        
        result.SetToHttpResponse(HttpContext, environment.IsProduction());
    }
    private async Task<Result<RefreshToken>> ValidateRefreshToken(RefreshToken? refreshToken)
    {
        var status = refreshTokenService.ValidateToken(refreshToken);

        if (status is RefreshTokenValidationStatus.Valid)
        {
            return refreshToken!;
        }
        if (status is RefreshTokenValidationStatus.NotFound)
        {
            return Failure.Unauthorized("Refresh token not found.");
        }
        await refreshTokenService.RevokeUserTokensAsync(refreshToken!.User);
        return Failure.Unauthorized("Refresh token is invalid.");
    }
}