namespace Veilora.Application.Settings;

public record PasswordResetSettings(string ClientBaseUrl, int TokenExpiryMinutes = 60);
