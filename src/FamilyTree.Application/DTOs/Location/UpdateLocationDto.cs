namespace FamilyTree.Application.DTOs.Location;

public record UpdateLocationDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}
