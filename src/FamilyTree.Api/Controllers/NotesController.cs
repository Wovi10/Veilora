using FamilyTree.Application.DTOs.Note;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTree.Api.Controllers;

[ApiController]
[Route("api/notes")]
[Authorize]
public class NotesController(INoteService noteService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await noteService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var note = await noteService.GetByIdAsync(id);
        return note is null ? NotFound() : Ok(note);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNoteDto dto)
    {
        var created = await noteService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNoteDto dto)
    {
        var updated = await noteService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await noteService.DeleteAsync(id);
        return NoContent();
    }
}
