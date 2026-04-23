using Veilora.Application.DTOs.Entity;

namespace Veilora.Application.DTOs.Search;

public record WorldSearchResultDto
{
    public IReadOnlyList<EntitySearchItemDto> Entities { get; init; } = [];
    public IReadOnlyList<EntityRefDto> Characters { get; init; } = [];
    public IReadOnlyList<EntityRefDto> Locations { get; init; } = [];
    public IReadOnlyList<EntityRefDto> Events { get; init; } = [];
}

public record EntitySearchItemDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Type { get; init; }
}
