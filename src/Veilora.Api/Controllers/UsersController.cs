using System.Security.Claims;
using Veilora.Application.DTOs.User;
using Veilora.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Veilora.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(IUserService userService) : ControllerBase
{
    private Guid? CurrentUserId =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    [HttpPut("me/display-name")]
    public async Task<IActionResult> UpdateDisplayName([FromBody] UpdateDisplayNameDto dto)
    {
        if (CurrentUserId is null) return Unauthorized();
        var updated = await userService.UpdateDisplayNameAsync(CurrentUserId.Value, dto);
        return Ok(updated);
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        if (CurrentUserId is null) return Unauthorized();
        await userService.ChangePasswordAsync(CurrentUserId.Value, dto);
        return NoContent();
    }
}
