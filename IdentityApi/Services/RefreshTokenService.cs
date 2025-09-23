using System.Security.Claims;
using System.Security.Cryptography;
using Core;
using Core.Services;
using Core.Utilities;
using IdentityApi.Models;
using IdentityApi.Services;

namespace IdentityApi;

internal interface IRefreshTokenService
{
    public Task<string>                 CreateAsync(User user);
    public RefreshTokenValidationStatus ValidateToken(RefreshToken? refreshToken);
    public Task<RefreshToken?>          GetTokenAsync(string token);
    public Task<RefreshToken>           RevokeTokenAsync(RefreshToken refreshToken);
    public Task                         RevokeUserTokensAsync(User user);
    public Task                         RevokeUserTokensAsync(ClaimsPrincipal principal);
}
internal enum RefreshTokenValidationStatus
{
    Expired,
    Revoked,
    NotFound,
    Valid
}
[AppService<IRefreshTokenService>]
internal class RefreshTokenService(
        IUserRepository userRepository,
        IAccountService accountService) : IRefreshTokenService
{
    public async Task<string> CreateAsync(User user)
    {
        var token     = GenerateRandomToken();
        var tokenHash = Hashing.Hash(token, Hashing.DefaultSHA256Rules);
        
        var newToken = new RefreshToken
        {
            Id        = Guid.NewGuid(),
            UserId    = user.Id,
            TokenHash = tokenHash,
            IssuedAt  = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(Constants.Tokens.REFRESH_TOKEN_EXPIRY_TIME),
            Revoked   = false,
            User      = user
        };
        await userRepository.AddRefreshTokenAsync(newToken);
        await userRepository.SaveChangesAsync();

        return token;
    }
    public async Task RevokeUserTokensAsync(ClaimsPrincipal principal)
    {
        var result = await accountService.GetUserAsync(principal);
        if (result.IsFailure)
        {
            return;
        }
        await RevokeUserTokensAsync(result.Value);
    }
    public async Task RevokeUserTokensAsync(User user)
    {
        await userRepository.DeleteUserRefreshTokensAsync(user);
    }
    public async Task<RefreshToken> RevokeTokenAsync(RefreshToken refreshToken)
    {
        refreshToken.Revoked = true;
        await userRepository.SaveChangesAsync();
        return refreshToken;
    }
    public async Task<RefreshTokenValidationStatus> ValidateToken(string tokenValue)
    {
        return ValidateToken(await GetTokenAsync(tokenValue));
    }
    public RefreshTokenValidationStatus ValidateToken(RefreshToken? refreshToken)
    {
        if (refreshToken is null)
        {
            return RefreshTokenValidationStatus.NotFound;
        }
        if (refreshToken.Revoked)
        {
            return RefreshTokenValidationStatus.Revoked;
        }
        if (refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            return RefreshTokenValidationStatus.Expired;
        }
        return RefreshTokenValidationStatus.Valid;
    }
    public async Task<RefreshToken?> GetTokenAsync(string token)
    {
        return await userRepository.GetRefreshTokenAsync(
            Hashing.Hash(token, Hashing.DefaultSHA256Rules));
    }
    private static string GenerateRandomToken(int size = 32)
    {
        var       bytes     = new byte[size];
        using var generator = RandomNumberGenerator.Create(); 
        
        generator.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}