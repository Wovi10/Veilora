namespace Veilora.Application.DTOs.Photo;

public record ReorderPhotosDto(IReadOnlyList<Guid> OrderedIds);
