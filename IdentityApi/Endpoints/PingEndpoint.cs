using FastEndpoints;

namespace IdentityApi.Endpoints;

public class PingEndpoint : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("/v1/ping");
        AllowAnonymous();
    }
    public override Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return Task.FromResult<IResult>(TypedResults.Ok("pong"));
    }
}