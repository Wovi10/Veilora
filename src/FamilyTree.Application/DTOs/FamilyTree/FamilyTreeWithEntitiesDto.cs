namespace FamilyTree.Application.DTOs.FamilyTree;

public record FamilyTreeWithEntitiesDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required Guid WorldId { get; init; }
    public required IEnumerable<CharacterInFamilyTreeDto> Characters { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
