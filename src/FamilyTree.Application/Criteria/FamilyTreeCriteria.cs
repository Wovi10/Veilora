namespace FamilyTree.Application.Criteria;

public record FamilyTreeCriteria(Guid WorldId, int Page = 1, int PageSize = 20)
    : PageCriteria(Page, PageSize);
