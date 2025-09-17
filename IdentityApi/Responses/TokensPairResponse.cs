namespace IdentityApi.Responses;

internal readonly record struct TokensPairResponse(string AccessToken, string RefreshToken);