namespace Veilora.Infrastructure.Email;

public record EmailSettings(
    string Host,
    int Port,
    string FromAddress,
    string FromName,
    string? Username,
    string? Password,
    bool UseSsl = true);
