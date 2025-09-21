using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Core.Middleware;

public static class IncomingRequestLoggerMiddlewareExtension
{
    public static IApplicationBuilder UseIncomingRequestLogger(this IApplicationBuilder builder)
        => builder.UseMiddleware<IncomingRequestLoggerMiddleware>();
}
public class IncomingRequestLoggerMiddleware(
    RequestDelegate next, ILogger<IncomingRequestLoggerMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        logger.LogInformation("=== Incoming Request ===");
        logger.LogInformation("Method: {RequestMethod}", context.Request.Method);
        logger.LogInformation("Path: {RequestPath}", context.Request.Path);
        logger.LogInformation("QueryString: {RequestQueryString}", context.Request.QueryString);
        logger.LogInformation("ContentType: {RequestContentType}", context.Request.ContentType);
        logger.LogInformation("Headers: {Join}", string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}={h.Value}")));
    
        if (context.Request.ContentLength > 0)
        {
            context.Request.EnableBuffering();
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            
            context.Request.Body.Position = 0;
            logger.LogInformation("Body: {Body}", body);
        }
        logger.LogInformation("========================");
    
        await next(context);
    }
}