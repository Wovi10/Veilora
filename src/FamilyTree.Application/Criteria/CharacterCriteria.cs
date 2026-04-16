namespace FamilyTree.Application.Criteria;

public record CharacterCriteria(
    Guid WorldId,
    int Page = 1,
    int PageSize = 20);
