namespace FamilyTree.Application.DTOs.FamilyTree;

public record UpdateFamilyTreeDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}
