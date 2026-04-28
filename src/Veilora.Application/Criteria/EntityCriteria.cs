namespace Veilora.Application.Criteria;

public record EntityCriteria(Guid WorldId, string? Type = null, int Page = 1, int PageSize = 20, string? Name = null)
    : PageCriteria(Page, PageSize);
