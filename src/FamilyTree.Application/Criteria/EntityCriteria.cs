namespace FamilyTree.Application.Criteria;

public record EntityCriteria(
    Guid WorldId,
    string? Type = null,
    int Page = 1,
    int PageSize = 20);
