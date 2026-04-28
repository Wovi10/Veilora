using Veilora.Application.Criteria;
using Veilora.Application.DTOs.Event;
using Veilora.Application.Exceptions;
using Veilora.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Veilora.Api.Controllers;

[ApiController]
[Route("api/events")]
[Authorize]
public class EventsController(IEventService eventService) : ControllerBase
{
    [HttpGet("world/{worldId:guid}")]
    public async Task<IActionResult> GetByWorld(
        Guid worldId,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? name)
    {
        if (page.HasValue || pageSize.HasValue || name is not null)
            return Ok(await eventService.GetPagedAsync(
                new EventCriteria(worldId, page ?? 1, pageSize ?? 20, name)));
        return Ok(await eventService.GetByWorldIdAsync(worldId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var ev = await eventService.GetByIdAsync(id);
        return ev is null ? NotFound() : Ok(ev);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
    {
        var created = await eventService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventDto dto)
    {
        var updated = await eventService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await eventService.DeleteAsync(id);
        return NoContent();
    }
}
