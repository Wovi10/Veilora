using FamilyTree.Api.Services;
using FamilyTree.Application.DTOs.Auth;
using FamilyTree.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTree.Api.Controllers;

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
            return Created(string.Empty, new AuthResponseDto(token, user.Email, user.DisplayName));
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
            return Unauthorized(new { message = "Invalid email or password." });

        var token = tokenService.GenerateToken(user);
        return Ok(new AuthResponseDto(token, user.Email, user.DisplayName));
    }
}
