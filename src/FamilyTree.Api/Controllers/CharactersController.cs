using FamilyTree.Application.Criteria;
using FamilyTree.Application.DTOs.Character;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTree.Api.Controllers;

[ApiController]
[Route("api/characters")]
[Authorize]
public class CharactersController(ICharacterService characterService, IRelationshipService relationshipService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByWorld(
        [FromQuery] Guid worldId,
        [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        if (page.HasValue || pageSize.HasValue)
            return Ok(await characterService.GetPagedAsync(
                new CharacterCriteria(worldId, page ?? 1, pageSize ?? 20)));
        return Ok(await characterService.GetByWorldIdAsync(worldId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var character = await characterService.GetByIdAsync(id);
        return character is null ? NotFound() : Ok(character);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q)) return BadRequest("Search term is required.");
        return Ok(await characterService.SearchAsync(q));
    }

    [HttpGet("{id:guid}/ancestors")]
    public async Task<IActionResult> GetAncestors(Guid id) =>
        Ok(await characterService.GetAncestorsAsync(id));

    [HttpGet("{id:guid}/descendants")]
    public async Task<IActionResult> GetDescendants(Guid id) =>
        Ok(await characterService.GetDescendantsAsync(id));

    [HttpGet("{id:guid}/relationships")]
    public async Task<IActionResult> GetRelationships(Guid id) =>
        Ok(await relationshipService.GetEntityRelationshipsAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCharacterDto dto)
    {
        var created = await characterService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCharacterDto dto)
    {
        var updated = await characterService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await characterService.DeleteAsync(id);
        return NoContent();
    }
}
