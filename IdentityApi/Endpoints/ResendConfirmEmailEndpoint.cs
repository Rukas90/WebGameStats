using Api.Services;
using IdentityApi.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace IdentityApi.Endpoints;

internal class ResendConfirmEmailEndpoint(
    IEmailConfirmationService emailConfirmationService,
    UserManager<User> userManager) 
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Post(routePatterns: "/v1/users/confirmEmail/resend");
    }
    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var user = await userManager.GetUserAsync(User);

        if (user == null)
        {
            return TypedResults.Problem(detail: "Failed to find user.");
        }
        await emailConfirmationService
            .SendConfirmationEmailAsync(user, HttpContext);

        return TypedResults.Ok("New confirmation email sent.");
    }
}