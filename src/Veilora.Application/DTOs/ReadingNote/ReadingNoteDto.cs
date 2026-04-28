namespace Veilora.Application.DTOs.ReadingNote;

public record ReadingNoteDto
{
    public required Guid Id { get; init; }
    public required Guid SessionId { get; init; }
    public required string Text { get; init; }
    public required string[] Tags { get; init; }
    public required DateTime CreatedAt { get; init; }
}
