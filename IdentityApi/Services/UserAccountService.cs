using System.Security.Claims;
using Core.Results;
using Core.Services;
using IdentityApi.Models;
using Microsoft.AspNetCore.Identity;
using ZiggyCreatures.Caching.Fusion;

namespace IdentityApi.Services;

public interface IAccountService
{
    public Task<bool> CheckPasswordAsync(User user, string password);
    public Task<Result<User>> GetByIdAsync(Guid id);
    public Task<Result<User>> GetByEmailAsync(string email);
    public Task<Result<User>> CreateUserAsync(User user, string password);
    public Task<bool> GetTwoFactorEnabledAsync(User user);
    public Task<Result<User>> GetUserAsync(ClaimsPrincipal principal);
    public Task<string> GenerateEmailConfirmationTokenAsync(User user);
    public Task<Result<NoContent>> ConfirmEmailAsync(User user, string code);
    public Task<IList<Claim>> GetClaimsAsync(User user);
    public Task<IList<string>> GetRolesAsync(User user);
}
[AppService<IAccountService>]
internal class AccountService(
    UserManager<User> userManager, IFusionCache cache) 
    : IAccountService
{
    public async Task<bool> CheckPasswordAsync(User user, string password) 
        => await userManager.CheckPasswordAsync(user, password);

    public async Task<Result<User>> GetByEmailAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user != null ? user : Failure.NotFound("User not found.");
    }
    public async Task<Result<User>> GetByIdAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        return user != null ? user : Failure.NotFound("User not found.");
    }
    public async Task<Result<User>> CreateUserAsync(User user, string password)
    {
        var result = await userManager.CreateAsync(user, password);
        return result.Succeeded
            ? user
            : Failure.Error(result.Errors);
    }
    public async Task<bool> GetTwoFactorEnabledAsync(User user)
        => await userManager.GetTwoFactorEnabledAsync(user);
    public async Task<Result<User>> GetUserAsync(ClaimsPrincipal principal)
    {
        var user = await userManager.GetUserAsync(principal);
        return user != null ? user : Failure.NotFound("User not found.");
    }
    public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        => await userManager.GenerateEmailConfirmationTokenAsync(user);

    public async Task<Result<NoContent>> ConfirmEmailAsync(User user, string code)
    {
        var result = await userManager.ConfirmEmailAsync(user, code);
        return result.Succeeded ? NoContent.Value : Failure.Error(result.Errors);
    }
    
    public async Task<IList<Claim>> GetClaimsAsync(User user)
        => await userManager.GetClaimsAsync(user);

    public async Task<IList<string>> GetRolesAsync(User user) 
        => await cache.GetOrSetAsync<IList<string>>(
            key:     $"UserRoles:{user.Id}",
            factory: async _ => await userManager.GetRolesAsync(user));
}