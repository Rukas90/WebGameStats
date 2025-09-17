namespace IdentityApi.Requests;

internal sealed class RegisterRequest
{
    public required string Email          { get; init; }
    public required string Username       { get; init; }
    public required string Password       { get; init; }
    public required string RepeatPassword { get; init; }
    public required string CaptchaToken   { get; init; }
}