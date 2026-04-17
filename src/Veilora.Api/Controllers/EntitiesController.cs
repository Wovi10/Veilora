using Veilora.Application.Criteria;
using Veilora.Application.DTOs.Entity;
using Veilora.Application.Exceptions;
using Veilora.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Veilora.Api.Controllers;

[ApiController]
[Route("api/entities")]
[Authorize]
public class EntitiesController(IEntityService entityService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? worldId,
        [FromQuery] string? type,
        [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        if (worldId.HasValue && (page.HasValue || pageSize.HasValue))
            return Ok(await entityService.GetPagedAsync(
                new EntityCriteria(worldId.Value, type, page ?? 1, pageSize ?? 20)));
        if (worldId.HasValue && type is not null)
            return Ok(await entityService.GetByWorldIdAndTypeAsync(worldId.Value, type));
        if (worldId.HasValue)
            return Ok(await entityService.GetByWorldIdAsync(worldId.Value));
        return Ok(await entityService.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await entityService.GetByIdAsync(id);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q)) return BadRequest("Search term is required.");
        return Ok(await entityService.SearchAsync(q));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEntityDto dto)
    {
        var created = await entityService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEntityDto dto)
    {
        var updated = await entityService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await entityService.DeleteAsync(id);
        return NoContent();
    }
}
