using Veilora.Domain.Entities;

namespace Veilora.Application.Repositories.Interfaces;

public interface ILanguageRepository : IRepository<Language>
{
    Task<IEnumerable<Language>> GetByWorldIdAsync(Guid worldId);
    Task<Language?> GetByNameAndWorldIdAsync(string name, Guid worldId);
}
