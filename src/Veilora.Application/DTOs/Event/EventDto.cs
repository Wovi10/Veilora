namespace Veilora.Application.DTOs.Event;

public record EventDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required Guid WorldId { get; init; }
    public string? Description { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
