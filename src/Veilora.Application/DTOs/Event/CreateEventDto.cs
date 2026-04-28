namespace Veilora.Application.DTOs.Event;

public record CreateEventDto
{
    public required string Name { get; init; }
    public required Guid WorldId { get; init; }
    public string? Description { get; init; }
}
