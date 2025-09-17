namespace IdentityApi.Requests;

internal sealed class LoginRequest
{
    public required string Email        { get; init; }
    public required string Password     { get; init; }
    public required string CaptchaToken { get; init; }
}