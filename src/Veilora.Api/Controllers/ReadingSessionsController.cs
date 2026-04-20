using System.Security.Claims;
using Veilora.Application.DTOs.ReadingNote;
using Veilora.Application.DTOs.ReadingSession;
using Veilora.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Veilora.Api.Controllers;

[ApiController]
[Route("api/reading-sessions")]
[Authorize]
public class ReadingSessionsController(IReadingSessionService readingSessionService) : ControllerBase
{
    private Guid? UserId =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    [HttpPost]
    public async Task<IActionResult> Start([FromBody] CreateReadingSessionDto dto)
    {
        if (UserId is not { } userId) return Unauthorized();
        var session = await readingSessionService.StartAsync(userId, dto);
        return Ok(session);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        if (UserId is not { } userId) return Unauthorized();
        var session = await readingSessionService.GetActiveAsync(userId);
        return session is null ? NoContent() : Ok(session);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (UserId is not { } userId) return Unauthorized();
        var sessions = await readingSessionService.GetAllAsync(userId);
        return Ok(sessions);
    }

    [HttpPost("{id:guid}/end")]
    public async Task<IActionResult> End(Guid id)
    {
        if (UserId is not { } userId) return Unauthorized();
        await readingSessionService.EndAsync(id, userId);
        return NoContent();
    }

    [HttpGet("{id:guid}/notes")]
    public async Task<IActionResult> GetNotes(Guid id)
    {
        if (UserId is not { } userId) return Unauthorized();
        var notes = await readingSessionService.GetNotesAsync(id, userId);
        return Ok(notes);
    }

    [HttpPost("{id:guid}/notes")]
    public async Task<IActionResult> AddNote(Guid id, [FromBody] CreateReadingNoteDto dto)
    {
        if (UserId is not { } userId) return Unauthorized();
        var note = await readingSessionService.AddNoteAsync(id, userId, dto);
        return Ok(note);
    }

    [HttpDelete("notes/{noteId:guid}")]
    public async Task<IActionResult> DeleteNote(Guid noteId)
    {
        if (UserId is not { } userId) return Unauthorized();
        await readingSessionService.DeleteNoteAsync(noteId, userId);
        return NoContent();
    }
}
