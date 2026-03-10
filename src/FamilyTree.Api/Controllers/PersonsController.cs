using FamilyTree.Application.DTOs.Person;
using FamilyTree.Application.DTOs.Relationship;
using FamilyTree.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTree.Api.Controllers;

[ApiController]
[Route("api/persons")]
public class PersonsController(IPersonService personService, IRelationshipService relationshipService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetAll()
    {
        var persons = await personService.GetAllAsync();
        return Ok(persons);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDto>> GetById(Guid id)
    {
        var person = await personService.GetByIdAsync(id);
        return Ok(person);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<PersonDto>>> Search([FromQuery] string? q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { message = "Search term is required." });

        var results = await personService.SearchAsync(q);
        return Ok(results);
    }

    [HttpGet("{id:guid}/ancestors")]
    [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetAncestors(Guid id)
    {
        var ancestors = await personService.GetAncestorsAsync(id);
        return Ok(ancestors);
    }

    [HttpGet("{id:guid}/descendants")]
    [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetDescendants(Guid id)
    {
        var descendants = await personService.GetDescendantsAsync(id);
        return Ok(descendants);
    }

    [HttpGet("{personId:guid}/relationships")]
    [ProducesResponseType(typeof(IEnumerable<RelationshipDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<RelationshipDto>>> GetRelationships(Guid personId)
    {
        var relationships = await relationshipService.GetPersonRelationshipsAsync(personId);
        return Ok(relationships);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonDto>> Create([FromBody] CreatePersonDto dto)
    {
        var person = await personService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDto>> Update(Guid id, [FromBody] UpdatePersonDto dto)
    {
        var person = await personService.UpdateAsync(id, dto);
        return Ok(person);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await personService.DeleteAsync(id);
        return NoContent();
    }
}
