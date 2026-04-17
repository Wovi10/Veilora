namespace Veilora.Application.DTOs.Entity;

public record UpdateEntityDto
{
    public required string Name { get; init; }
    public required string Type { get; init; }
    public string? Description { get; init; }
}
