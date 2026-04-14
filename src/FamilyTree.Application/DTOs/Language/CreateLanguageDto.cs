namespace FamilyTree.Application.DTOs.Language;

public record CreateLanguageDto
{
    public required string Name { get; init; }
    public required Guid WorldId { get; init; }
}
