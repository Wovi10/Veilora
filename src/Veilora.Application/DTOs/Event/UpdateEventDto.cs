namespace Veilora.Application.DTOs.Event;

public record UpdateEventDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}
