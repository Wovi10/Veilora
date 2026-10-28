using Veilora.Api.Services;
using Veilora.Application.DTOs.Auth;
using Veilora.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Veilora.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, ITokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var user = await authService.RegisterAsync(dto);
            var token = tokenService.GenerateToken(user);
            return Created(string.Empty, new AuthResponseDto(user.Id, token, user.Email, user.DisplayName));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var user = await authService.ValidateAsync(dto);
        if (user is null)
            return Unauthorized(new { message = "Invalid username/email or password." });

        var token = tokenService.GenerateToken(user);
        return Ok(new AuthResponseDto(user.Id, token, user.Email, user.DisplayName));
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        await authService.ForgotPasswordAsync(dto);
        return Ok(new { message = "If an account exists for that email, a reset link has been sent." });
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        await authService.ResetPasswordAsync(dto);
        return Ok(new { message = "Your password has been reset." });
    }
}
