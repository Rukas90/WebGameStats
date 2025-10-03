using Api.Services;
using FastEndpoints;
using Flurl;

namespace IdentityApi.Endpoints;

internal class ConfirmEmailEndpoint(
    IEmailConfirmationService     emailConfirmationService,
    ILogger<ConfirmEmailEndpoint> logger,
    IConfiguration                configuration)
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get(routePatterns: "/v1/users/confirmEmail");
    }
    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var userId = Query<Guid>("userId");
        var code   = Query<string>("code");
        
        var result = await emailConfirmationService
            .ConfirmEmailAsync(userId, code, ct);

        result.Perform(
            onSuccess: () => logger.LogInformation("Email confirmed for user {UserId}.",         userId),
            onFailure: () => logger.LogInformation("Failed to confirm email for user {UserId}.", userId));
        
        return result.Match<IResult>(
            onSuccess: message => TypedResults.Redirect(
                configuration["Hosts:Client"]!.AppendPathSegment("emailConfirmed")),
            onFailure: TypedResults.Problem);
    }
}