using FamilyTree.Application.DTOs.Relationship;
using FamilyTree.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTree.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/relationships")]
public class RelationshipsController(IRelationshipService relationshipService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RelationshipDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RelationshipDto>>> GetAll()
    {
        var relationships = await relationshipService.GetAllAsync();
        return Ok(relationships);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RelationshipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RelationshipDto>> GetById(Guid id)
    {
        var relationship = await relationshipService.GetByIdAsync(id);
        return Ok(relationship);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RelationshipDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RelationshipDto>> Create([FromBody] CreateRelationshipDto dto)
    {
        var relationship = await relationshipService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = relationship.Id }, relationship);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(RelationshipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RelationshipDto>> Update(Guid id, [FromBody] UpdateRelationshipDto dto)
    {
        var relationship = await relationshipService.UpdateAsync(id, dto);
        return Ok(relationship);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await relationshipService.DeleteAsync(id);
        return NoContent();
    }
}
