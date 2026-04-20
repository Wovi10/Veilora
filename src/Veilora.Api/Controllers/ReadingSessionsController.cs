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

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent()
    {
        if (UserId is not { } userId) return Unauthorized();
        var session = await readingSessionService.GetCurrentAsync(userId);
        return session is null ? NoContent() : Ok(session);
    }

    [HttpPost("{id:guid}/pause")]
    public async Task<IActionResult> Pause(Guid id)
    {
        if (UserId is not { } userId) return Unauthorized();
        await readingSessionService.PauseAsync(id, userId);
        return NoContent();
    }

    [HttpPost("{id:guid}/resume")]
    public async Task<IActionResult> Resume(Guid id)
    {
        if (UserId is not { } userId) return Unauthorized();
        await readingSessionService.ResumeAsync(id, userId);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Clear(Guid id)
    {
        if (UserId is not { } userId) return Unauthorized();
        var worldId = await readingSessionService.ClearAsync(id, userId);
        return Ok(new { worldId });
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
