namespace Veilora.Application.DTOs.Note;

public record CreateNoteDto
{
    public required string Content { get; init; }
    public Guid? WorldId { get; init; }
    public Guid? EntityId { get; init; }
}
