using Api.Services;
using FastEndpoints;
using IdentityApi.Mapping;
using IdentityApi.Services;
using Microsoft.AspNetCore.Antiforgery;
using LoginRequest = IdentityApi.Requests.LoginRequest;

namespace IdentityApi.Endpoints;

internal class LoginEndpoint(
    ILoginService       loginService, 
    IWebHostEnvironment environment, 
    IHCaptchaService    hCaptchaService,
    IIPAddressService   ipAddressService,
    IAntiforgery        antiforgery)
    : Endpoint<LoginRequest, IResult>
{
    public override void Configure()
    {
        Post(routePatterns: "/v1/auth/login");
        AllowAnonymous();
        Throttle(hitLimit: 60, durationSeconds: 240);
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
        tokens.SetToHttpResponse(HttpContext, environment.IsProduction());
        
        return TypedResults.Ok(new
        {
            message     = "Login successful",
            csrfToken   = antiforgery.GetAndStoreTokens(HttpContext).RequestToken!,
            redirectUrl = "/"
        });
    }
}