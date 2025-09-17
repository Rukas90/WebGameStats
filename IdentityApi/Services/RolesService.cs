using Core.Results;
using Core.Services;
using IdentityApi.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityApi.Services;

internal interface IRolesService
{
    public Task<Result<string>> AddRoleToUserAsync(User      user, string role, CancellationToken cancellationToken);
    public Task<Result<string>> RemoveRoleFromUserAsync(User user, string role, CancellationToken cancellationToken);
    public Task<bool>           DoesRoleExistAsync(string    role);
}
[AppService<IRolesService>]
internal class RolesService(
    RoleManager<UserRole> roleManager, 
    IUserStore<User>      userStore,
    ILogger<RolesService> logger) : IRolesService
{
    public async Task<Result<string>> AddRoleToUserAsync(
        User user, string role, CancellationToken cancellationToken)
    {
        if (!await EnsureRoleExistsAsync(role))
        {
            return Failure.NotFound("Role not found.");
        }
        if (await DoesUserHaveRoleAsync(user, role, cancellationToken))
        {
            return "User already has role.";
        }
        await RoleStore.AddToRoleAsync(user, NormalizeRole(role), cancellationToken);
        return "Role added to user.";
    }
    public async Task<Result<string>> RemoveRoleFromUserAsync(
        User user, string role, CancellationToken cancellationToken)
    {
        if (!await EnsureRoleExistsAsync(role))
        {
            return Failure.NotFound("Role not found.");
        }
        if (!await DoesUserHaveRoleAsync(user, role, cancellationToken))
        {
            return "User already does not have a role.";
        }
        await RoleStore.RemoveFromRoleAsync(user, NormalizeRole(role), cancellationToken);
        return "Role removed from user.";
    }
    private async Task<bool> EnsureRoleExistsAsync(string role)
    {
        if (await DoesRoleExistAsync(role))
        {
            return true;
        }
        var result = await roleManager.CreateAsync(new UserRole(role));
        
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                logger.LogWarning("Role creation error for '{Role}': {Error}", role, error.Description);
            }
        }
        return result.Succeeded;
    }
    public async Task<bool> DoesRoleExistAsync(string role)
    {
        return await roleManager.RoleExistsAsync(NormalizeRole(role));
    }
    private async Task<bool> DoesUserHaveRoleAsync(
        User user, string role, CancellationToken cancellationToken)
    {
        var roles = await GetRolesAsync(user, cancellationToken);
        return roles.Contains(role);
    }
    private async Task<IList<string>> GetRolesAsync(
        User user, CancellationToken cancellationToken) 
        => await RoleStore.GetRolesAsync(user, cancellationToken);
    
    private static string NormalizeRole(string role) 
        => role.ToUpper();
    
    private IUserRoleStore<User> RoleStore => (IUserRoleStore<User>)userStore;
}