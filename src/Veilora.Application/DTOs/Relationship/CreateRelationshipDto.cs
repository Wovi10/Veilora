namespace Veilora.Application.DTOs.Relationship;

public record CreateRelationshipDto
{
    public required Guid Character1Id { get; init; }
    public required Guid Character2Id { get; init; }
    public required string RelationshipType { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public string? Notes { get; init; }
}
