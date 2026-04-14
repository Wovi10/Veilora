using FamilyTree.Application.DTOs.Language;

namespace FamilyTree.Application.Services.Interfaces;

public interface ILanguageService
{
    Task<IEnumerable<LanguageDto>> GetByWorldIdAsync(Guid worldId);
    Task<LanguageDto> GetOrCreateAsync(string name, Guid worldId);
}
