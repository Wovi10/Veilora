namespace FamilyTree.Application.DTOs.Relationship;

public record UpdateRelationshipDto
{
    public Guid Person1Id { get; init; }
    public Guid Person2Id { get; init; }
    public string RelationshipType { get; init; } = string.Empty;
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public string? Notes { get; init; }
}