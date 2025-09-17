namespace IdentityApi.Responses;

internal readonly record struct AuthInfoResponse(AuthStatusResponse Status, ProfileResponse? Profile);