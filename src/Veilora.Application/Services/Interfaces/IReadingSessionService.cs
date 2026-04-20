using Veilora.Application.DTOs.ReadingNote;
using Veilora.Application.DTOs.ReadingSession;

namespace Veilora.Application.Services.Interfaces;

public interface IReadingSessionService
{
    Task<ReadingSessionDto> StartAsync(Guid userId, CreateReadingSessionDto dto);
    Task<ReadingSessionDto?> GetCurrentAsync(Guid userId);
    Task PauseAsync(Guid sessionId, Guid userId);
    Task ResumeAsync(Guid sessionId, Guid userId);
    Task<Guid> ClearAsync(Guid sessionId, Guid userId);
    Task<IEnumerable<ReadingNoteDto>> GetNotesAsync(Guid sessionId, Guid userId);
    Task<ReadingNoteDto> AddNoteAsync(Guid sessionId, Guid userId, CreateReadingNoteDto dto);
    Task DeleteNoteAsync(Guid noteId, Guid userId);
}
