using FamilyTree.Application.DTOs.Person;

namespace FamilyTree.Application.Services.Interfaces;

public interface IPersonService
{
    Task<IEnumerable<PersonDto>> GetAllAsync();
    Task<PersonDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<PersonDto>> GetPersonsByTreeIdAsync(Guid treeId);
    Task<PersonDto> CreateAsync(CreatePersonDto dto);
    Task<PersonDto> UpdateAsync(Guid id, UpdatePersonDto dto);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<PersonDto>> SearchAsync(string searchTerm);
    Task<IEnumerable<PersonDto>> GetAncestorsAsync(Guid personId);
    Task<IEnumerable<PersonDto>> GetDescendantsAsync(Guid personId);
}

