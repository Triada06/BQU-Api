

using BGU.Application.Common;
using BGU.Core.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BGU.Application.Services;

public class MailKitEmailSender(IOptions<EmailSettings> options) : IEmailSender<AppUser>
{
    private readonly EmailSettings _settings = options.Value;

    public async Task SendConfirmationLinkAsync(AppUser user, string email, string confirmationLink)
        => await SendAsync(email, "Confirm your email", 
            $"<a href='{confirmationLink}'>Confirm</a>");

    public async Task SendPasswordResetLinkAsync(AppUser user, string email, string resetLink)
        => await SendAsync(email, "Reset your password", $"Click to reset your password: <a href='{resetLink}'>Reset</a>");

    public async Task SendPasswordResetCodeAsync(AppUser user, string email, string resetCode)
        => await SendAsync(email, "Your password reset code", $"Your reset code: <b>{resetCode}</b>");

    private async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}