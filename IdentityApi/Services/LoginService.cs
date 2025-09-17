using Core.Results;
using Core.Services;
using IdentityApi;
using IdentityApi.Commands;
using IdentityApi.Responses;
using IdentityApi.Models;
using Microsoft.AspNetCore.Identity;

namespace Api.Services;

internal interface ILoginService
{
    public Task<Result<TokensPairResponse>> LoginAsync(
        LoginCommand command, CancellationToken cancellationToken);
}
[AppService<ILoginService>]
internal class LoginService(
    UserManager<User>    userManager,
    IJwtService          jwtService,
    IRefreshTokenService refreshTokenService) : ILoginService
{
    public async Task<Result<TokensPairResponse>> LoginAsync(
        LoginCommand command, CancellationToken cancellationToken)
    {
        var validation = await ValidateAuthentication(command);
        
        if (!validation.IsAuthenticated)
        {
            return validation.Problem!;
        }
        cancellationToken.ThrowIfCancellationRequested();
        var user = validation.User!;
        
        return new TokensPairResponse(
            AccessToken:  await jwtService.Generate(user),
            RefreshToken: await refreshTokenService.CreateAsync(user));
    }
    private async Task<UserAuthValidationResult> ValidateAuthentication(LoginCommand command)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        
        if (user is null)
        {
            return Failure.BadRequest("Invalid credentials");
        }
        var validPassword = await userManager.CheckPasswordAsync(user, command.Password);
        
        if (!validPassword)
        {
            return Failure.BadRequest("Invalid credentials");
        }
        if (!await userManager.GetTwoFactorEnabledAsync(user))
        {
            return UserAuthValidationResult.Success(user);
        }
        return user;
    }
}