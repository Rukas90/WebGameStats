using Core;
using FastEndpoints;
using IdentityApi.Responses;
using IdentityApi.Services;

namespace IdentityApi.Endpoints;

internal class AuthInfoEndpoint(
    IAuthService authService,
    IUserService userService)
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get(routePatterns: "/v1/auth/info");
        AllowAnonymous();
    }
    public override Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var authStatus = authService.GetAuthStatus(
            HttpContext.Request.Cookies[Constants.Tokens.ACCESS_TOKEN_COOKIE_NAME]);
    
        var includeProfile = Query<bool>(paramName: "includeProfile", isRequired: false);
        ProfileResponse? profile = null;
        
        if (includeProfile)
        {
            var profileResult = userService.GetProfileFromPrincipal(User);
            profile = profileResult.IsSuccess ? profileResult.Value : null;
        }
        return Task.FromResult<IResult>(TypedResults.Ok(
            new AuthInfoResponse
        {
            Status  = authStatus,
            Profile = profile
        }));
    }
}