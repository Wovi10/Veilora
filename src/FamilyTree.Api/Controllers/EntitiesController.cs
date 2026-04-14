using FamilyTree.Application.DTOs.Entity;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTree.Api.Controllers;

[ApiController]
[Route("api/entities")]
[Authorize]
public class EntitiesController(IEntityService entityService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await entityService.GetAllAsync());

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
