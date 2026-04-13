namespace FamilyTree.Application.DTOs.FamilyTree;

public record CreateFamilyTreeDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required Guid WorldId { get; init; }
}
