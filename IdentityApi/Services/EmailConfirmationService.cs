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
    UserManager<User>   userManager,
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
        var user = await userManager.FindByIdAsync(userId.ToString());
        
        if (user is null)
        {
            return Failure.Unauthorized(message: "User not found!");
        }
        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Failure.Unauthorized(message: "Code is invalid!");
        }
        cancellationToken.ThrowIfCancellationRequested();
        
        IdentityResult result = await userManager.ConfirmEmailAsync(user, code);
        
        if (!result.Succeeded)
        {
            return Failure.Unauthorized(message: "Code is invalid!");
        }
        await rolesService.RemoveRoleFromUserAsync(user, Constants.Roles.GUEST, cancellationToken);
        await rolesService.AddRoleToUserAsync(user, Constants.Roles.ADMIN, cancellationToken);
        
        return "Thank you for confirming your email.";
    }
    public async Task SendConfirmationEmailAsync(User user, HttpContext context)
    {
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        
        var url = new UriBuilder
        {
            Scheme = context.Request.Scheme,
            Host   = context.Request.Host.Host,
            Port   = context.Request.Host.Port ?? -1,
            Path   = "/v1/users/confirmEmail",
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