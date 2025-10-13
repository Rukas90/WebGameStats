using FastEndpoints;
using IdentityApi.Responses;
using IdentityApi.Services;

namespace IdentityApi.Endpoints;

internal class ProfileEndpoint(IUserService userService) 
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("/v1/users/profile");
        Throttle(hitLimit: 120, durationSeconds: 120);
    }
    public override Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return Task.FromResult(userService.GetProfileFromPrincipal(User)
            .Match<IResult>(
            onSuccess: TypedResults.Ok<ProfileResponse>,
            onFailure: TypedResults.Problem));
    }
}