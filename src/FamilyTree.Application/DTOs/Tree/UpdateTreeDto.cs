namespace FamilyTree.Application.DTOs.Tree;

public record UpdateTreeDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}