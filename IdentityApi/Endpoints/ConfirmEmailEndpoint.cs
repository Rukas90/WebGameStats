using Api.Services;
using FastEndpoints;

namespace IdentityApi.Endpoints;

internal class ConfirmEmailEndpoint(IEmailConfirmationService emailConfirmationService)
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get(routePatterns: "/v1/identity/users/confirmEmail");
    }
    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var userId = Query<Guid>("userId");
        var code   = Query<string>("code");
        
        var result = await emailConfirmationService
            .ConfirmEmailAsync(userId, code, ct);
        
        return result.Match<IResult>(
            onSuccess: message => TypedResults.Redirect("http://127.0.0.1:5173/emailConfirmed"),
            onFailure: TypedResults.Problem);
    }
}