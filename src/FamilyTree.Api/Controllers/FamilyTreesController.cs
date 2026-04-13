using FamilyTree.Application.DTOs.FamilyTree;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTree.Api.Controllers;

[ApiController]
[Route("api/family-trees")]
[Authorize]
public class FamilyTreesController(IFamilyTreeService familyTreeService, IEntityService entityService) : ControllerBase
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

    [HttpGet("{familyTreeId:guid}/entities")]
    public async Task<IActionResult> GetEntities(Guid familyTreeId) =>
        Ok(await entityService.GetEntitiesByFamilyTreeIdAsync(familyTreeId));

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

    [HttpPost("{familyTreeId:guid}/entities/{entityId:guid}")]
    public async Task<IActionResult> AddEntity(Guid familyTreeId, Guid entityId)
    {
        await familyTreeService.AddEntityToFamilyTreeAsync(familyTreeId, entityId);
        return NoContent();
    }

    [HttpDelete("{familyTreeId:guid}/entities/{entityId:guid}")]
    public async Task<IActionResult> RemoveEntity(Guid familyTreeId, Guid entityId)
    {
        await familyTreeService.RemoveEntityFromFamilyTreeAsync(familyTreeId, entityId);
        return NoContent();
    }

    [HttpPut("{familyTreeId:guid}/entities/{entityId:guid}/position")]
    public async Task<IActionResult> UpdateEntityPosition(Guid familyTreeId, Guid entityId, [FromBody] UpdateEntityPositionDto dto)
    {
        await familyTreeService.UpdateEntityPositionAsync(familyTreeId, entityId, dto.X, dto.Y);
        return NoContent();
    }
}
