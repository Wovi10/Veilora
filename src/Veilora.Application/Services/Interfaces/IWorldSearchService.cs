using Veilora.Application.Criteria;
using Veilora.Application.DTOs.World;

namespace Veilora.Application.Services.Interfaces;

public interface IWorldSearchService
{
    Task<WorldSearchResultDto> SearchAsync(WorldSearchCriteria criteria);
}
