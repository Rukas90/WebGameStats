using System.Security.Claims;
using Core.Results;
using Core.Services;
using IdentityApi.Responses;

namespace IdentityApi.Services;

public interface IUserMetadataService
{
    public Task<Result<ProfileMetadata>> GetUserProfileAsync(Guid userId);
}
internal interface IUserService
{
    public Result<ProfileResponse> GetProfileFromPrincipal(ClaimsPrincipal principal);
}
[AppService<IUserService>, AppService<IUserMetadataService>]
internal class UserService(IUserRepository repository) 
    : IUserService, IUserMetadataService
{
    public async Task<Result<ProfileMetadata>> GetUserProfileAsync(Guid userId)
    {
        var user = await repository.GetByIdAsync(userId);

        if (user == null)
        {
            return Failure.NotFound("User by id is not found.");
        }
        return new ProfileMetadata(user.Id, user.UserName);
    }
    public Result<ProfileResponse> GetProfileFromPrincipal(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return Failure.Unauthorized("User is not authenticated.");
        }
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        var username    = principal.FindFirst(ClaimTypes.Name)?.Value;
        
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username))
        {
            return Failure.Unauthorized("User is not authenticated.");
        }
        return new ProfileResponse(username, email);
    }
}