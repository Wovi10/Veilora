using FamilyTree.Application.DTOs.Person;
using FamilyTree.Application.DTOs.Relationship;
using FamilyTree.Application.DTOs.Tree;
using FamilyTree.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTree.Api.Controllers;

[ApiController]
[Route("api/trees")]
public class TreesController(
    ITreeService treeService,
    IPersonService personService,
    IRelationshipService relationshipService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TreeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TreeDto>>> GetAll()
    {
        var trees = await treeService.GetAllAsync();
        return Ok(trees);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TreeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TreeDto>> GetById(Guid id)
    {
        var tree = await treeService.GetByIdAsync(id);
        return Ok(tree);
    }

    [HttpGet("{id:guid}/details")]
    [ProducesResponseType(typeof(TreeWithPersonsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TreeWithPersonsDto>> GetDetails(Guid id)
    {
        var tree = await treeService.GetTreeWithPersonsAsync(id);
        return Ok(tree);
    }

    [HttpGet("{treeId:guid}/persons")]
    [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetPersons(Guid treeId)
    {
        var persons = await personService.GetPersonsByTreeIdAsync(treeId);
        return Ok(persons);
    }

    [HttpGet("{treeId:guid}/relationships")]
    [ProducesResponseType(typeof(IEnumerable<RelationshipDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<RelationshipDto>>> GetRelationships(Guid treeId)
    {
        var relationships = await relationshipService.GetRelationshipsByTreeIdAsync(treeId);
        return Ok(relationships);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TreeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TreeDto>> Create([FromBody] CreateTreeDto dto)
    {
        var tree = await treeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = tree.Id }, tree);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TreeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TreeDto>> Update(Guid id, [FromBody] UpdateTreeDto dto)
    {
        var tree = await treeService.UpdateAsync(id, dto);
        return Ok(tree);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await treeService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{treeId:guid}/persons/{personId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddPerson(Guid treeId, Guid personId)
    {
        await treeService.AddPersonToTreeAsync(treeId, personId);
        return NoContent();
    }

    [HttpDelete("{treeId:guid}/persons/{personId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemovePerson(Guid treeId, Guid personId)
    {
        await treeService.RemovePersonFromTreeAsync(treeId, personId);
        return NoContent();
    }
}
