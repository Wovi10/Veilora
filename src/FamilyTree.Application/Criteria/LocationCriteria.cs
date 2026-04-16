namespace FamilyTree.Application.Criteria;

public record LocationCriteria(
    Guid WorldId,
    int Page = 1,
    int PageSize = 20);
