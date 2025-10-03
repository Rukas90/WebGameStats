using Api.Services;
using FastEndpoints;
using IdentityApi.Mapping;
using IdentityApi.Services;
using RegisterRequest = IdentityApi.Requests.RegisterRequest;

namespace IdentityApi.Endpoints;

internal class RegisterEndpoint(
    IRegistrationService registrationService, 
    IWebHostEnvironment  environment,
    IHCaptchaService     hCaptchaService,
    IIPAddressService    ipAddressService)
    : Endpoint<RegisterRequest, IResult>
{
    public override void Configure()
    {
        Post(routePatterns: "/v1/auth/register");
        AllowAnonymous();
    }
    public override async Task<IResult> ExecuteAsync(
        RegisterRequest request, CancellationToken ct)
    {
        var captchaValid = await hCaptchaService.VerifyAsync(
            request.CaptchaToken, ipAddressService.GetClientIP(HttpContext.Request));

        if (!captchaValid)
        {
            return TypedResults.Problem("Captcha validation failed");
        }
        var result = await registrationService.RegisterAsync(request.ToCommand(), HttpContext, ct);
        
        if (result.IsFailure)
        {
            return TypedResults.Problem(result.Problem);
        }
        var tokens = result.Value;
        
        tokens.SetToHttpResponse(HttpContext, environment.IsProduction());
        
        return TypedResults.Ok(new
        {
            message     = "Registration successful",
            redirectUrl = "/"
        });
    }
}