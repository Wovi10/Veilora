using FamilyTree.Application.DTOs.Relationship;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTree.Api.Controllers;

[ApiController]
[Route("api/relationships")]
[Authorize]
public class RelationshipsController(IRelationshipService relationshipService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await relationshipService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var relationship = await relationshipService.GetByIdAsync(id);
        return relationship is null ? NotFound() : Ok(relationship);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRelationshipDto dto)
    {
        var created = await relationshipService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRelationshipDto dto)
    {
        var updated = await relationshipService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await relationshipService.DeleteAsync(id);
        return NoContent();
    }
}
