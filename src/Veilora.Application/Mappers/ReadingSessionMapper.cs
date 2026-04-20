using Veilora.Application.DTOs.ReadingNote;
using Veilora.Application.DTOs.ReadingSession;
using Veilora.Domain.Entities;

namespace Veilora.Application.Mappers;

public static class ReadingSessionMapper
{
    public static ReadingSessionDto ToDto(ReadingSession session, string worldName, int noteCount) => new()
    {
        Id = session.Id,
        WorldId = session.WorldId,
        WorldName = worldName,
        StartedAt = session.StartedAt,
        EndedAt = session.EndedAt,
        NoteCount = noteCount,
    };

    public static ReadingNoteDto ToNoteDto(ReadingNote note) => new()
    {
        Id = note.Id,
        SessionId = note.SessionId,
        Text = note.Text,
        Tags = note.Tags,
        CreatedAt = note.CreatedAt,
    };
}
