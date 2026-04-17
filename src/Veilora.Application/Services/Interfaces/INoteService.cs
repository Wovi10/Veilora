using Veilora.Application.DTOs.Note;

namespace Veilora.Application.Services.Interfaces;

public interface INoteService
{
    Task<IEnumerable<NoteDto>> GetAllAsync();
    Task<NoteDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<NoteDto>> GetByWorldIdAsync(Guid worldId);
    Task<IEnumerable<NoteDto>> GetByEntityIdAsync(Guid entityId);
    Task<NoteDto> CreateAsync(CreateNoteDto dto);
    Task<NoteDto> UpdateAsync(Guid id, UpdateNoteDto dto);
    Task DeleteAsync(Guid id);
}
