namespace Veilora.Application.Criteria;

public record WorldSearchCriteria(Guid WorldId, string Name, int Page = 1, int PageSize = 20)
    : PageCriteria(Page, PageSize);
