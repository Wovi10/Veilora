namespace FamilyTree.Application.DTOs.DateSuffix;

public record CreateDateSuffixDto
{
    public required string Name { get; init; }
    public required string Abbreviation { get; init; }
    public int Order { get; init; }
    public bool IsDefault { get; init; }
    public required Guid WorldId { get; init; }
}
