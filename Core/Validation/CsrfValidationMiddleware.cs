using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Core.Csrf;

public static class CsrfValidationMiddlewareExtension
{
    public static IApplicationBuilder UseCsrfValidation(this IApplicationBuilder builder)
        => builder.UseMiddleware<CsrfValidationMiddleware>();
}
public class CsrfValidationMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        bool invalid = false;
        
        try
        {
            if (!IsSecureMethod(context))
            {
                return;
            }
            var cookie = context.Request.Cookies[Constants.Tokens.CSRF_TOKEN_COOKIE_NAME];
            var header = context.Request.Headers[Constants.Tokens.CSRF_TOKEN_HEADER_NAME].FirstOrDefault();
        
            if (string.IsNullOrEmpty(cookie) || string.IsNullOrEmpty(header) || cookie != header)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Invalid CSRF token");

                invalid = true;
            }
        }
        finally
        {
            if (!invalid)
            {
                await next(context);
            }
        }
    }
    private static bool IsSecureMethod(HttpContext context)
        => HttpMethods.IsPost(context.Request.Method)   ||
           HttpMethods.IsPut(context.Request.Method)    ||
           HttpMethods.IsDelete(context.Request.Method) ||
           HttpMethods.IsPatch(context.Request.Method);
}