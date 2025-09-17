using Core;
using Core.Config.Jwt;
using Core.Services;
using IdentityApi.Responses;
using Microsoft.IdentityModel.Tokens;

namespace IdentityApi.Services;

internal interface IAuthService
{
    public AuthStatusResponse GetAuthStatus(string? token);
}
[AppService<IAuthService>]
internal class AuthService(
    IJwtService jwtService, IConfiguration configuration) : IAuthService
{
    private readonly TokenValidationParameters validationParameters 
        = JwtConfig.CreateTokenValidationParameters(configuration);
    
    public AuthStatusResponse GetAuthStatus(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return AuthStatusResponse.Unauthenticated();
        }
        var principal = jwtService.Validate(
            token, 
            validationParameters,
            out _);
        
        if (principal is null)
        {
            return AuthStatusResponse.Unauthenticated();
        }
        return new AuthStatusResponse
        {
            isAuthenticated = principal.Identity?.IsAuthenticated ?? false,
            isEmailVerified = bool.Parse(principal.FindFirst(Constants.CustomClaims.EMAIL_VERIFIED)?.Value ?? "false")
        };
    }
}