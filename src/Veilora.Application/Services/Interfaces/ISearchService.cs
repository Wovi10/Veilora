using Veilora.Application.DTOs.Search;

namespace Veilora.Application.Services.Interfaces;

public interface ISearchService
{
    Task<WorldSearchResultDto> SearchWorldAsync(Guid worldId, string query);
}
