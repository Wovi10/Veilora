namespace FamilyTree.Application.DTOs.DateSuffix;

public record UpdateDateSuffixDto
{
    public required string Name { get; init; }
    public required string Abbreviation { get; init; }
    public int Order { get; init; }
    public bool IsDefault { get; init; }
}
