using FastEndpoints;
using Microsoft.AspNetCore.Antiforgery;

namespace IdentityApi.Endpoints;

public class AntiforgeryEndpoint (
    IAntiforgery                 antiforgery, 
    ILogger<AntiforgeryEndpoint> logger) 
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get(routePatterns: "/v1/csrf/token");
        AllowAnonymous();
    }
    public override Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var csrfTokens = antiforgery.GetAndStoreTokens(HttpContext);
        
        logger.LogInformation("Generated CSRF Request: {name}", csrfTokens.RequestToken!);
        
        return Task.FromResult<IResult>(TypedResults.Ok(new
        {
            message   = "CSRF token refreshed",
            csrfToken = csrfTokens.RequestToken!
        }));
    }
}