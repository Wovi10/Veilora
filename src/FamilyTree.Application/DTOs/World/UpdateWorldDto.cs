namespace FamilyTree.Application.DTOs.World;

public record UpdateWorldDto
{
    public required string Name { get; init; }
    public string? Author { get; init; }
    public string? Description { get; init; }
}
