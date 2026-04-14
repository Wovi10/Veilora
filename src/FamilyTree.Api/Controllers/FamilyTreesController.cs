using FamilyTree.Application.DTOs.FamilyTree;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTree.Api.Controllers;

[ApiController]
[Route("api/family-trees")]
[Authorize]
public class FamilyTreesController(IFamilyTreeService familyTreeService, ICharacterService characterService, IRelationshipService relationshipService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await familyTreeService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var tree = await familyTreeService.GetByIdAsync(id);
        return tree is null ? NotFound() : Ok(tree);
    }

    [HttpGet("{id:guid}/details")]
    public async Task<IActionResult> GetWithEntities(Guid id)
    {
        var tree = await familyTreeService.GetFamilyTreeWithEntitiesAsync(id);
        return tree is null ? NotFound() : Ok(tree);
    }

    [HttpGet("{familyTreeId:guid}/characters")]
    public async Task<IActionResult> GetCharacters(Guid familyTreeId) =>
        Ok(await characterService.GetByFamilyTreeIdAsync(familyTreeId));

    [HttpGet("{familyTreeId:guid}/relationships")]
    public async Task<IActionResult> GetRelationships(Guid familyTreeId) =>
        Ok(await relationshipService.GetRelationshipsByFamilyTreeIdAsync(familyTreeId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFamilyTreeDto dto)
    {
        var created = await familyTreeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFamilyTreeDto dto)
    {
        var updated = await familyTreeService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await familyTreeService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{familyTreeId:guid}/characters/{characterId:guid}")]
    public async Task<IActionResult> AddCharacter(Guid familyTreeId, Guid characterId)
    {
        await familyTreeService.AddCharacterToFamilyTreeAsync(familyTreeId, characterId);
        return NoContent();
    }

    [HttpDelete("{familyTreeId:guid}/characters/{characterId:guid}")]
    public async Task<IActionResult> RemoveCharacter(Guid familyTreeId, Guid characterId)
    {
        await familyTreeService.RemoveCharacterFromFamilyTreeAsync(familyTreeId, characterId);
        return NoContent();
    }

    [HttpPut("{familyTreeId:guid}/characters/{characterId:guid}/position")]
    public async Task<IActionResult> UpdateCharacterPosition(Guid familyTreeId, Guid characterId, [FromBody] UpdateEntityPositionDto dto)
    {
        await familyTreeService.UpdateCharacterPositionAsync(familyTreeId, characterId, dto.X, dto.Y);
        return NoContent();
    }
}
