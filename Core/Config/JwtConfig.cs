using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Core.Config.Jwt;

public static class JwtConfig
{
    public static TokenValidationParameters CreateTokenValidationParameters(
        IConfiguration configuration)
    {
        return new TokenValidationParameters
        {
            ValidIssuer              = configuration["Jwt:Issuer"],
            ValidAudience            = configuration["Jwt:Audience"],
            IssuerSigningKey         = GetSigningSecurityKey(configuration["Jwt:Key"]!),
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true
        };
    }
    public static SecurityKey GetSigningSecurityKey(string key)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }
}