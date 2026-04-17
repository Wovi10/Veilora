namespace Veilora.Application.DTOs.Location;

public record CreateLocationDto
{
    public required string Name { get; init; }
    public required Guid WorldId { get; init; }
    public string? Description { get; init; }
}
