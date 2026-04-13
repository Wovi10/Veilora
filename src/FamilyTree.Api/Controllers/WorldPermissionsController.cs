using System.Security.Claims;
using FamilyTree.Application.DTOs.WorldPermission;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTree.Api.Controllers;

[ApiController]
[Route("api/worlds/{worldId:guid}/permissions")]
[Authorize]
public class WorldPermissionsController(IWorldPermissionService permissionService, IWorldService worldService, IUserRepository userRepository) : ControllerBase
{
    private Guid? CurrentUserId =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid worldId)
    {
        if (!await IsOwnerAsync(worldId)) return Forbid();
        return Ok(await permissionService.GetByWorldAsync(worldId));
    }

    [HttpPost]
    public async Task<IActionResult> AddByEmail(Guid worldId, [FromBody] AddPermissionByEmailDto dto)
    {
        if (!await IsOwnerAsync(worldId)) return Forbid();
        var user = await userRepository.GetByEmailAsync(dto.Email);
        if (user is null) return NotFound(new { message = "No user found with that email address." });
        var result = await permissionService.UpsertAsync(worldId, new UpsertWorldPermissionDto(user.Id, dto.CanEdit));
        return Ok(result);
    }

    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> Upsert(Guid worldId, Guid userId, [FromBody] UpsertWorldPermissionDto dto)
    {
        if (!await IsOwnerAsync(worldId)) return Forbid();
        var result = await permissionService.UpsertAsync(worldId, dto with { UserId = userId });
        return Ok(result);
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> Delete(Guid worldId, Guid userId)
    {
        if (!await IsOwnerAsync(worldId)) return Forbid();
        await permissionService.DeleteAsync(worldId, userId);
        return NoContent();
    }

    private async Task<bool> IsOwnerAsync(Guid worldId)
    {
        var world = await worldService.GetByIdAsync(worldId);
        return world?.CreatedById == CurrentUserId;
    }
}

public record AddPermissionByEmailDto(string Email, bool CanEdit);
