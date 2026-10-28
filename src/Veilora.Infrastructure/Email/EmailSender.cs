using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using Veilora.Application.Services.Interfaces;

namespace Veilora.Infrastructure.Email;

public class EmailSender(EmailSettings settings, ILogger<EmailSender> logger) : IEmailSender
{
    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink, int expiryMinutes)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(settings.FromName, settings.FromAddress));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Reset your Veilora password";
        message.Body = new TextPart("plain")
        {
            Text = $"""
                    We received a request to reset the password for your Veilora account.

                    Reset your password using the link below:
                    {resetLink}

                    This link will expire in {expiryMinutes} minutes. If you didn't request this, you can safely ignore this email.
                    """
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(
            settings.Host,
            settings.Port,
            settings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto);

        if (!string.IsNullOrEmpty(settings.Username) && !string.IsNullOrEmpty(settings.Password))
            await client.AuthenticateAsync(settings.Username, settings.Password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        logger.LogInformation("Password reset email sent to {Email}", toEmail);
    }
}
