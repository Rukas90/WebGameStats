namespace IdentityApi.Responses;

public readonly record struct ProfileMetadata(Guid UserId, string? Username);