using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core;
using Core.Config.Jwt;
using IdentityApi.Models;
using Core.Services;
using IdentityApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ZiggyCreatures.Caching.Fusion;

namespace IdentityApi;

internal interface IJwtService
{
    public Task<string>     Generate(User user);
    public ClaimsPrincipal? Validate(string token, TokenValidationParameters validationParameters, out SecurityToken? validatedToken);
}
[AppService<IJwtService>]
internal class JwtService(
    IConfiguration    configuration, 
    IAccountService   accountService,
    IFusionCache      cache) : IJwtService
{
    private static readonly TimeSpan TOKEN_EXPIRATION_TIME 
        = TimeSpan.FromHours(1);
    
    public async Task<string> Generate(User user)
    {
        var userClaims = await accountService.GetClaimsAsync(user);
        var userRoles = await accountService.GetRolesAsync(user);
        
        var claims = GetClaims(user);
        
        claims.AddRange(userClaims);
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var secretKey = configuration["Jwt:Key"];
        
        var key         = JwtConfig.GetSigningSecurityKey(secretKey!);
        var credentials = new SigningCredentials(key, algorithm: SecurityAlgorithms.HmacSha512);
        
        var expires = DateTime.UtcNow.Add(TOKEN_EXPIRATION_TIME);

        var descriptor = new JwtSecurityToken(
            issuer:             configuration["Jwt:Issuer"],
            audience:           configuration["Jwt:Audience"],
            claims:             claims,
            expires:            expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(descriptor);
    }

    public ClaimsPrincipal? Validate(
        string token, TokenValidationParameters validationParameters, out SecurityToken? validatedToken)
    {
        var handler = new JwtSecurityTokenHandler();
        try
        {
            return handler.ValidateToken(token, validationParameters, out validatedToken);
        }
        catch (SecurityTokenException)
        {
            validatedToken = null;
            return null;
        }
    }
    private static List<Claim> GetClaims(User user) =>
    [
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Email, user.Email!),
        new(ClaimTypes.Name, user.UserName!),
        new(Constants.CustomClaims.EMAIL_VERIFIED, user.EmailConfirmed.ToString())
    ];
}