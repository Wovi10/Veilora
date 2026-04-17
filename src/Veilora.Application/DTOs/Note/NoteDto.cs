namespace Veilora.Application.DTOs.Note;

public record NoteDto
{
    public required Guid Id { get; init; }
    public required string Content { get; init; }
    public Guid? WorldId { get; init; }
    public Guid? EntityId { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
