namespace Veilora.Application.DTOs.Entity;

public record CreateEntityDto
{
    public required string Name { get; init; }
    public required string Type { get; init; }
    public required Guid WorldId { get; init; }
    public string? Description { get; init; }
}
