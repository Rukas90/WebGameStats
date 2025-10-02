using FastEndpoints;
using Microsoft.AspNetCore.Antiforgery;

namespace IdentityApi.Endpoints;

public class AntiforgeryEndpoint (
    IAntiforgery antiforgery) 
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get(routePatterns: "/v1/identity/csrf/token");
        AllowAnonymous();
    }
    public override Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var csrfTokens = antiforgery.GetAndStoreTokens(HttpContext);
        
        return Task.FromResult<IResult>(TypedResults.Ok(new
        {
            message   = "CSRF token refreshed",
            csrfToken = csrfTokens.RequestToken!
        }));
    }
}