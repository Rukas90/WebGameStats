namespace IdentityApi.Commands;

internal record RegisterCommand(string Email, string Username, string Password);