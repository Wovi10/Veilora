using FamilyTree.Application.DTOs.Language;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services.Interfaces;
using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Services;

public class LanguageService(ILanguageRepository languageRepository) : ILanguageService
{
    public async Task<IEnumerable<LanguageDto>> GetByWorldIdAsync(Guid worldId)
    {
        var languages = await languageRepository.GetByWorldIdAsync(worldId);
        return languages.Select(l => new LanguageDto(l.Id, l.Name, l.WorldId));
    }

    public async Task<LanguageDto> GetOrCreateAsync(string name, Guid worldId)
    {
        var existing = await languageRepository.GetByNameAndWorldIdAsync(name.Trim(), worldId);
        if (existing is not null)
            return new LanguageDto(existing.Id, existing.Name, existing.WorldId);

        var language = new Language { Name = name.Trim(), WorldId = worldId };
        await languageRepository.AddAsync(language);
        await languageRepository.SaveChangesAsync();
        return new LanguageDto(language.Id, language.Name, language.WorldId);
    }
}
