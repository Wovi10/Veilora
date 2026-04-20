namespace Veilora.Application.DTOs.World;

public record WorldSearchItemDto(Guid Id, string Name);

public record WorldSearchResultDto
{
    public required IReadOnlyList<WorldSearchItemDto> Characters  { get; init; }
    public required IReadOnlyList<WorldSearchItemDto> Locations   { get; init; }
    public required IReadOnlyList<WorldSearchItemDto> Groups      { get; init; }
    public required IReadOnlyList<WorldSearchItemDto> Events      { get; init; }
    public required IReadOnlyList<WorldSearchItemDto> Concepts    { get; init; }
    public required IReadOnlyList<WorldSearchItemDto> FamilyTrees { get; init; }
}
