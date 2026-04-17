using Veilora.Application.DTOs.Note;
using Veilora.Domain.Entities;

namespace Veilora.Application.Mappers;

public static class NoteMapper
{
    public static NoteDto ToDto(Note note) => new()
    {
        Id = note.Id,
        Content = note.Content,
        WorldId = note.WorldId,
        EntityId = note.EntityId,
        CreatedAt = note.CreatedAt,
        UpdatedAt = note.UpdatedAt
    };

    public static Note ToEntity(CreateNoteDto dto) => new()
    {
        Content = dto.Content,
        WorldId = dto.WorldId,
        EntityId = dto.EntityId
    };

    public static void UpdateEntity(UpdateNoteDto dto, Note note)
    {
        note.Content = dto.Content;
        note.WorldId = dto.WorldId;
        note.EntityId = dto.EntityId;
    }
}
