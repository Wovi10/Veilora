using System.Security.Claims;
using Veilora.Application.Criteria;
using Veilora.Application.DTOs.World;
using Veilora.Application.Exceptions;
using Veilora.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Veilora.Api.Controllers;

[ApiController]
[Route("api/worlds")]
[Authorize]
public class WorldsController(IWorldService worldService, IWorldSearchService worldSearchService) : ControllerBase
{
    private Guid? CurrentUserId =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (CurrentUserId is null) return Unauthorized();
        return Ok(await worldService.GetAllByUserAsync(CurrentUserId.Value));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var world = await worldService.GetByIdAsync(id);
        return world is null ? NotFound() : Ok(world);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorldDto dto)
    {
        var created = await worldService.CreateAsync(dto, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWorldDto dto)
    {
        var world = await worldService.GetByIdAsync(id);
        if (world is null) return NotFound();
        if (world.CreatedById != CurrentUserId) return Forbid();

        var updated = await worldService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpPut("{id:guid}/owner")]
    public async Task<IActionResult> TransferOwnership(Guid id, [FromBody] TransferOwnershipDto dto)
    {
        var world = await worldService.GetByIdAsync(id);
        if (world is null) return NotFound();
        if (world.CreatedById != CurrentUserId) return Forbid();

        var found = await worldService.TransferOwnershipAsync(id, dto.Email);
        return found ? NoContent() : NotFound(new { message = "No user found with that email address." });
    }

    [HttpGet("{id:guid}/search")]
    public async Task<IActionResult> Search(Guid id, [FromQuery] string name, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(name)) return BadRequest("Search term is required.");
        var criteria = new WorldSearchCriteria(id, name, page, pageSize);
        var result = await worldSearchService.SearchAsync(criteria);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var world = await worldService.GetByIdAsync(id);
        if (world is null) return NotFound();
        if (world.CreatedById != CurrentUserId) return Forbid();

        await worldService.DeleteAsync(id);
        return NoContent();
    }
}

public record TransferOwnershipDto(string Email);
