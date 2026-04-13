namespace FamilyTree.Application.DTOs.Relationship;

public record UpdateRelationshipDto
{
    public required Guid Entity1Id { get; init; }
    public required Guid Entity2Id { get; init; }
    public required string RelationshipType { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public string? Notes { get; init; }
}
