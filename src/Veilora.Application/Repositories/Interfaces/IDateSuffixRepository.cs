using Veilora.Domain.Entities;

namespace Veilora.Application.Repositories.Interfaces;

public interface IDateSuffixRepository : IRepository<DateSuffix>
{
    Task<IEnumerable<DateSuffix>> GetByWorldIdAsync(Guid worldId);
    Task<DateSuffix?> GetDefaultForWorldAsync(Guid worldId);
    Task ClearDefaultForWorldAsync(Guid worldId);
}
