using Veilora.Application.DTOs.ReadingNote;
using Veilora.Application.DTOs.ReadingSession;

namespace Veilora.Application.Services.Interfaces;

public interface IReadingSessionService
{
    Task<ReadingSessionDto> StartAsync(Guid userId, CreateReadingSessionDto dto);
    Task<ReadingSessionDto?> GetActiveAsync(Guid userId);
    Task<IEnumerable<ReadingSessionDto>> GetAllAsync(Guid userId);
    Task EndAsync(Guid sessionId, Guid userId);
    Task<IEnumerable<ReadingNoteDto>> GetNotesAsync(Guid sessionId, Guid userId);
    Task<ReadingNoteDto> AddNoteAsync(Guid sessionId, Guid userId, CreateReadingNoteDto dto);
    Task DeleteNoteAsync(Guid noteId, Guid userId);
}
