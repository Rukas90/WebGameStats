namespace IdentityApi.Responses;

internal readonly record struct AuthStatusResponse
{
    public bool isAuthenticated { get; init; }
    public bool isEmailVerified { get; init; }

    public static AuthStatusResponse Unauthenticated() => new();
}