using Veilora.Application.DTOs.Language;

namespace Veilora.Application.Services.Interfaces;

public interface ILanguageService
{
    Task<IEnumerable<LanguageDto>> GetByWorldIdAsync(Guid worldId);
    Task<LanguageDto> GetOrCreateAsync(string name, Guid worldId);
}
