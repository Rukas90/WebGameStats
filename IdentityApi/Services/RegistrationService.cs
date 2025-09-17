using Core;
using Core.Results;
using Core.Services;
using IdentityApi;
using IdentityApi.Models;
using IdentityApi.Commands;
using IdentityApi.Responses;
using IdentityApi.Services;
using Microsoft.AspNetCore.Identity;

namespace Api.Services;

internal interface IRegistrationService
{
    public Task<Result<TokensPairResponse>> RegisterAsync(
        RegisterCommand command, HttpContext context, CancellationToken cancellationToken);
}
[AppService<IRegistrationService>]
internal class RegistrationService(
    UserManager<User>         userManager,
    IUserStore<User>          userStore,
    IEmailConfirmationService emailConfirmation,
    IJwtService               jwtService,
    IRefreshTokenService      refreshTokenService,
    IRolesService             rolesService) : IRegistrationService
{
    public async Task<Result<TokensPairResponse>> RegisterAsync(
        RegisterCommand command, HttpContext context, CancellationToken cancellationToken)
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("Requires a user store with email support.");
        }
        var email = command.Email;
        var emailStore = (IUserEmailStore<User>)userStore;
        var user       = new User();
        
        await userStore.SetUserNameAsync(user, command.Username, cancellationToken);
        await emailStore.SetEmailAsync(user, email, cancellationToken);
        
        cancellationToken.ThrowIfCancellationRequested();
        
        var result = await userManager.CreateAsync(user, command.Password);
        
        if (!result.Succeeded)
        {
            return Failure.Error(result);
        }
        await rolesService.AddRoleToUserAsync(user, Constants.Roles.GUEST, cancellationToken);
        await emailConfirmation.SendConfirmationEmailAsync(user, context);
        
        return new TokensPairResponse(
            AccessToken:  await jwtService.Generate(user),
            RefreshToken: await refreshTokenService.CreateAsync(user));
    }
}