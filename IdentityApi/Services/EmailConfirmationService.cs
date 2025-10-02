using System.Text;
using System.Text.Encodings.Web;
using Core;
using Core.Results;
using Core.Services;
using IdentityApi.Models;
using IdentityApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using MimeKit;

namespace Api.Services;

internal interface IEmailConfirmationService
{
    public Task<Result<string>> ConfirmEmailAsync(
        Guid userId, string? code, CancellationToken cancellationToken);
    
    public Task SendConfirmationEmailAsync(User user, HttpContext context);
}
[AppService<IEmailConfirmationService>]
internal class EmailConfirmationService(
    IAccountService     accountService,
    IEmailSenderService senderService,
    IRolesService       rolesService) : IEmailConfirmationService
{
    public async Task<Result<string>> ConfirmEmailAsync(
        Guid userId, string? code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(code))
        {
            return Failure.Unauthorized(message: "Code is invalid!");
        }
        var userResult = await accountService.GetByIdAsync(userId);
        
        if (userResult.IsFailure)
        {
            return Failure.Unauthorized(message: "User not found!");
        }
        var user = userResult.Value;
        
        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Failure.Unauthorized(message: "Code is invalid!");
        }
        cancellationToken.ThrowIfCancellationRequested();
        
        if ((await accountService.ConfirmEmailAsync(user, code)).IsFailure)
        {
            return Failure.Unauthorized(message: "Code is invalid!");
        }
        await rolesService.RemoveRoleFromUserAsync(user, Constants.Roles.GUEST, cancellationToken);
        await rolesService.AddRoleToUserAsync(user, Constants.Roles.ADMIN, cancellationToken);
        
        return "Thank you for confirming your email.";
    }
    public async Task SendConfirmationEmailAsync(User user, HttpContext context)
    {
        var code = await accountService.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        
        var url = new UriBuilder
        {
            Scheme = context.Request.Scheme,
            Host   = context.Request.Host.Host,
            Port   = context.Request.Host.Port ?? -1,
            Path   = "/v1/identity/users/confirmEmail",
            Query  = $"userId={Uri.EscapeDataString(user.Id.ToString())}&code={Uri.EscapeDataString(code)}"
        };
        await senderService.SendMessage(
            new MailboxAddress(user.UserName, user.Email),
            "Confirm your email",
            new BodyBuilder
            {
                HtmlBody = HtmlEncoder.Default.Encode(url.ToString())
            });
    }
}