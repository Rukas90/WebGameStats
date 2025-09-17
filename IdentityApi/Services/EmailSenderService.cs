using Core.Services;
using MailKit.Net.Smtp;
using MimeKit;

namespace IdentityApi.Services;

internal interface IEmailSenderService
{
    public Task SendMessage(MailboxAddress recipient, string subject, BodyBuilder body);
}
[AppService<IEmailSenderService>]
internal class EmailSenderService(IConfiguration configuration) : IEmailSenderService
{
    private const string SENDER_MAILBOX_ADDRESS_NAME = "QuizGame";
    
    public async Task SendMessage(MailboxAddress recipient, string subject, BodyBuilder body)
    {
        using var message = new MimeMessage();
        
        message.From.Add(GetSenderAddress());
        message.To.Add(recipient);
        
        message.Subject = subject;
        message.Body    = body.ToMessageBody();
        
        using var smtp = new SmtpClient();
        
        await smtp.ConnectAsync(GetSenderHost(), GetSenderPort(), MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(GetSenderEmail(), GetSenderPassword());
        
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
    private MailboxAddress GetSenderAddress() => new(SENDER_MAILBOX_ADDRESS_NAME, GetSenderEmail());
    private string GetSenderHost() => configuration["MailSender:Host"]!;
    private int GetSenderPort() => configuration.GetSection("MailSender:Port").Get<int>();
    private string GetSenderEmail() => configuration["MailSender:Email"]!;
    private string GetSenderPassword() => configuration["MailSender:Password"]!;
}