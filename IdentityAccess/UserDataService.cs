using Core.Results;
using IdentityApi.Services;

namespace IdentityAccess;

public interface IUserDataService
{
    public Task<Result<UserMetadata>> GetUserMetadataAsync(Guid userId);
}
internal class UserDataService(
    IUserMetadataService userMetadataService) : IUserDataService
{
    public async Task<Result<UserMetadata>> GetUserMetadataAsync(Guid userId)
    {
        var result = await userMetadataService.GetUserProfileAsync(userId);

        return result.Match<Result<UserMetadata>>(
            onSuccess: metadata => new UserMetadata(metadata.UserId, metadata.Username),
            onFailure: problem => problem);
    }
}