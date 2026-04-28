namespace Veilora.Application.DTOs.ReadingSession;

public record ReadingSessionDto
{
    public required Guid Id { get; init; }
    public required Guid WorldId { get; init; }
    public required string WorldName { get; init; }
    public required DateTime StartedAt { get; init; }
    public DateTime? EndedAt { get; init; }
    public required int NoteCount { get; init; }
    public required bool IsActive { get; init; }
}
