namespace Veilora.Application.DTOs.Photo;

public record PhotoDto(
    Guid Id,
    Guid EntityId,
    string EntityType,
    string Url,
    int DisplayOrder,
    DateTime CreatedAt,
    DateTime UpdatedAt);
