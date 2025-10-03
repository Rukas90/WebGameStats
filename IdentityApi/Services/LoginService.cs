using Core.Results;
using Core.Services;
using IdentityApi;
using IdentityApi.Commands;
using IdentityApi.Responses;
using IdentityApi.Services;

namespace Api.Services;

internal interface ILoginService
{
    public Task<Result<TokensPairResponse>> LoginAsync(
        LoginCommand command, CancellationToken cancellationToken);
}
[AppService<ILoginService>]
internal class LoginService(
    IAccountService       accountService,
    IJwtService           jwtService,
    IRefreshTokenService  refreshTokenService,
    ILogger<LoginService> logger) : ILoginService
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
        var result = await accountService.GetByEmailAsync(command.Email);
        
        if (result.IsFailure)
        {
            logger.LogWarning("Failed login attempt. Reason: {Details}.", result.Problem.Detail);
            return Failure.BadRequest("Invalid credentials");
        }
        var user = result.Value;
        var validPassword = await accountService.CheckPasswordAsync(user, command.Password);
        
        if (!validPassword)
        {
            logger.LogWarning("Failed login attempt. Subject: {UserId}, Reason: {Details}.", user.Id, "Invalid password.");
            return Failure.BadRequest("Invalid credentials");
        }
        return user;
    }
}