using FastEndpoints;
using IdentityApi.Services;

namespace IdentityApi.Endpoints;

internal class ProfileEndpoint(IUserService userService) 
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("/v1/users/profile");
    }
    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return userService.GetProfileFromPrincipal(User).Match<IResult>(
            onSuccess: TypedResults.Ok,
            onFailure: TypedResults.Problem);
    }
}