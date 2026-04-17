namespace Veilora.Application.Criteria;

public record LocationCriteria(Guid WorldId, int Page = 1, int PageSize = 20)
    : PageCriteria(Page, PageSize);
