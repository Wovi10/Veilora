using System.Security.Claims;
using Veilora.Application.DTOs.Photo;
using Veilora.Application.Services.Interfaces;
using Veilora.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Veilora.Api.Controllers;

[ApiController]
[Route("api/photos/{entityType}/{entityId:guid}")]
[Authorize]
public class PhotosController(IPhotoService photoService) : ControllerBase
{
    private Guid? CurrentUserId =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    [HttpGet]
    public async Task<IActionResult> GetByEntity(string entityType, Guid entityId)
    {
        if (!Enum.TryParse<PhotoTargetType>(entityType, ignoreCase: true, out var type))
            return BadRequest("Unknown entity type.");
        return Ok(await photoService.GetByEntityAsync(type, entityId));
    }

    [HttpPost]
    public async Task<IActionResult> Add(string entityType, Guid entityId, [FromBody] CreatePhotoDto dto)
    {
        if (!Enum.TryParse<PhotoTargetType>(entityType, ignoreCase: true, out var type))
            return BadRequest("Unknown entity type.");
        if (CurrentUserId is not { } userId) return Unauthorized();
        var result = await photoService.AddAsync(type, entityId, userId, dto);
        return CreatedAtAction(nameof(GetByEntity), new { entityType, entityId }, result);
    }

    [HttpPut("{photoId:guid}")]
    public async Task<IActionResult> Update(string entityType, Guid entityId, Guid photoId, [FromBody] UpdatePhotoDto dto)
    {
        if (!Enum.TryParse<PhotoTargetType>(entityType, ignoreCase: true, out _))
            return BadRequest("Unknown entity type.");
        if (CurrentUserId is not { } userId) return Unauthorized();
        var result = await photoService.UpdateAsync(photoId, userId, dto);
        return Ok(result);
    }

    [HttpDelete("{photoId:guid}")]
    public async Task<IActionResult> Delete(string entityType, Guid entityId, Guid photoId)
    {
        if (!Enum.TryParse<PhotoTargetType>(entityType, ignoreCase: true, out _))
            return BadRequest("Unknown entity type.");
        if (CurrentUserId is not { } userId) return Unauthorized();
        await photoService.DeleteAsync(photoId, userId);
        return NoContent();
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder(string entityType, Guid entityId, [FromBody] ReorderPhotosDto dto)
    {
        if (!Enum.TryParse<PhotoTargetType>(entityType, ignoreCase: true, out var type))
            return BadRequest("Unknown entity type.");
        if (CurrentUserId is not { } userId) return Unauthorized();
        await photoService.ReorderAsync(type, entityId, userId, dto);
        var updated = await photoService.GetByEntityAsync(type, entityId);
        return Ok(updated);
    }
}
