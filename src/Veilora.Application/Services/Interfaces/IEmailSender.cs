namespace Veilora.Application.Services.Interfaces;

public interface IEmailSender
{
    Task SendPasswordResetEmailAsync(string toEmail, string resetLink, int expiryMinutes);
}
