using Core.Config.Jwt;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services.Extensions;

public static class ServicesConfigExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = JwtConfig.CreateTokenValidationParameters(configuration);
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[Constants.Tokens.ACCESS_TOKEN_COOKIE_NAME];
                        return Task.CompletedTask;
                    }
                };
            })
            /*.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId        = configuration["Authentication:Google:ClientId"];
                options.ClientSecret    = configuration["Authentication:Google:ClientSecret"];
                options.CallbackPath = "/auth/google/callback";
                
                options.Scope.Add("email");
                options.Scope.Add("profile");
                
                options.SaveTokens = true;
            })*/;
        return services;
    }
    public static IServiceCollection ExtendProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance =
                    $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

                var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
            };
        });
        return services;
    }
    public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration, out string policyName)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        policyName = Constants.Cors.POLICY_NAME;
        
        services.AddCors(options =>
        {
            options.AddPolicy(name: Constants.Cors.POLICY_NAME, policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
        return services;
    }
}