namespace FamilyTree.Application.DTOs.DateSuffix;

public record CreateDateSuffixDto
{
    public required string Name { get; init; }
    public required string Abbreviation { get; init; }
    public long AnchorYear { get; init; }
    public decimal Scale { get; init; } = 1m;
    public bool IsReversed { get; init; }
    public bool IsDefault { get; init; }
    public required Guid WorldId { get; init; }
}
