using Api.Services;
using FastEndpoints;
using IdentityApi.Services;

namespace IdentityApi.Endpoints;

internal class ResendConfirmEmailEndpoint(
    IEmailConfirmationService emailConfirmationService,
    IAccountService accountService) 
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Post(routePatterns: "/v1/identity/users/confirmEmail/resend");
        EnableAntiforgery();
    }
    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var result = await accountService.GetUserAsync(User);

        if (result.IsFailure)
        {
            return TypedResults.Problem(detail: "Failed to find user.");
        }
        var user = result.Value;
        
        await emailConfirmationService
            .SendConfirmationEmailAsync(user, HttpContext);

        return TypedResults.Ok("New confirmation email sent.");
    }
}