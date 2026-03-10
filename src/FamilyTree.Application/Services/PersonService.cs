using FamilyTree.Application.DTOs.Person;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Mappers;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services.Interfaces;

namespace FamilyTree.Application.Services;

public class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;
    private readonly ITreeRepository _treeRepository;

    public PersonService(IPersonRepository personRepository, ITreeRepository treeRepository)
    {
        _personRepository = personRepository;
        _treeRepository = treeRepository;
    }

    public async Task<IEnumerable<PersonDto>> GetAllAsync()
    {
        var persons = await _personRepository.GetAllAsync();
        return persons.Select(PersonMapper.ToDto);
    }

    public async Task<PersonDto?> GetByIdAsync(Guid id)
    {
        var person = await _personRepository.GetByIdAsync(id);
        if (person == null)
        {
            throw new NotFoundException($"Person with ID {id} not found");
        }

        return PersonMapper.ToDto(person);
    }

    public async Task<IEnumerable<PersonDto>> GetPersonsByTreeIdAsync(Guid treeId)
    {
        var persons = await _personRepository.GetPersonsByTreeIdAsync(treeId);
        return persons.Select(PersonMapper.ToDto);
    }

    public async Task<PersonDto> CreateAsync(CreatePersonDto dto)
    {
        var person = PersonMapper.ToEntity(dto);
        await _personRepository.AddAsync(person);
        await _personRepository.SaveChangesAsync();

        return PersonMapper.ToDto(person);
    }

    public async Task<PersonDto> UpdateAsync(Guid id, UpdatePersonDto dto)
    {
        var person = await _personRepository.GetByIdAsync(id);
        if (person == null)
        {
            throw new NotFoundException($"Person with ID {id} not found");
        }

        PersonMapper.UpdateEntity(dto, person);
        await _personRepository.UpdateAsync(person);
        await _personRepository.SaveChangesAsync();

        return PersonMapper.ToDto(person);
    }

    public async Task DeleteAsync(Guid id)
    {
        var person = await _personRepository.GetByIdAsync(id);
        if (person == null)
        {
            throw new NotFoundException($"Person with ID {id} not found");
        }

        await _personRepository.DeleteAsync(person);
        await _personRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<PersonDto>> SearchAsync(string searchTerm)
    {
        var persons = await _personRepository.SearchAsync(searchTerm);
        return persons.Select(PersonMapper.ToDto);
    }

    public async Task<IEnumerable<PersonDto>> GetAncestorsAsync(Guid personId)
    {
        var person = await _personRepository.GetByIdAsync(personId);
        if (person == null)
        {
            throw new NotFoundException($"Person with ID {personId} not found");
        }

        var ancestors = await _personRepository.GetAncestorsAsync(personId);
        return ancestors.Select(PersonMapper.ToDto);
    }

    public async Task<IEnumerable<PersonDto>> GetDescendantsAsync(Guid personId)
    {
        var person = await _personRepository.GetByIdAsync(personId);
        if (person == null)
        {
            throw new NotFoundException($"Person with ID {personId} not found");
        }

        var descendants = await _personRepository.GetDescendantsAsync(personId);
        return descendants.Select(PersonMapper.ToDto);
    }
}

