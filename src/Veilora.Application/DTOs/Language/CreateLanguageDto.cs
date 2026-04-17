namespace Veilora.Application.DTOs.Language;

public record CreateLanguageDto
{
    public required string Name { get; init; }
    public required Guid WorldId { get; init; }
}
