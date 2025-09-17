using Api.Services;
using FastEndpoints;
using IdentityApi.Mapping;
using IdentityApi.Services;
using LoginRequest = IdentityApi.Requests.LoginRequest;

namespace IdentityApi.Endpoints;

internal class LoginEndpoint(
    ILoginService       loginService, 
    IWebHostEnvironment environment, 
    IHCaptchaService    hCaptchaService,
    IIPAddressService   ipAddressService)
    : Endpoint<LoginRequest, IResult>
{
    public override void Configure()
    {
        Post(routePatterns: "/v1/auth/login");
        AllowAnonymous();
    }
    public override async Task<IResult> ExecuteAsync(
        LoginRequest request, CancellationToken ct)
    {
        var captchaValid = await hCaptchaService.VerifyAsync(
            request.CaptchaToken, ipAddressService.GetClientIP(HttpContext.Request));

        if (!captchaValid)
        {
            return TypedResults.Problem("Captcha validation failed");
        }
        var result = await loginService.LoginAsync(request.ToCommand(), ct);

        if (result.IsFailure)
        {
            return TypedResults.Problem(result.Problem);
        }
        var tokens = result.Value;
        tokens.SetToHttpResponse(HttpContext.Response, environment.IsProduction());
        
        return TypedResults.Ok("Login successful");
    }
}