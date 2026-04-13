using FamilyTree.Application.DTOs.World;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTree.Api.Controllers;

[ApiController]
[Route("api/worlds")]
[Authorize]
public class WorldsController(IWorldService worldService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await worldService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var world = await worldService.GetByIdAsync(id);
        return world is null ? NotFound() : Ok(world);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorldDto dto)
    {
        var created = await worldService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWorldDto dto)
    {
        var updated = await worldService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await worldService.DeleteAsync(id);
        return NoContent();
    }
}
