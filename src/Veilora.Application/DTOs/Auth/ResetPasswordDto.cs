namespace Veilora.Application.DTOs.Auth;

public record ResetPasswordDto(string Token, string NewPassword);
