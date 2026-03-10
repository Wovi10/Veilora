using FamilyTree.Application.DTOs.Relationship;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Mappers;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services.Interfaces;

namespace FamilyTree.Application.Services;

public class RelationshipService : IRelationshipService
{
    private readonly IRelationshipRepository _relationshipRepository;
    private readonly IPersonRepository _personRepository;

    public RelationshipService(IRelationshipRepository relationshipRepository, IPersonRepository personRepository)
    {
        _relationshipRepository = relationshipRepository;
        _personRepository = personRepository;
    }

    public async Task<IEnumerable<RelationshipDto>> GetAllAsync()
    {
        var relationships = await _relationshipRepository.GetAllAsync();
        return relationships.Select(RelationshipMapper.ToDto);
    }

    public async Task<RelationshipDto?> GetByIdAsync(Guid id)
    {
        var relationship = await _relationshipRepository.GetByIdAsync(id);
        if (relationship == null)
        {
            throw new NotFoundException($"Relationship with ID {id} not found");
        }

        return RelationshipMapper.ToDto(relationship);
    }

    public async Task<IEnumerable<RelationshipDto>> GetRelationshipsByTreeIdAsync(Guid treeId)
    {
        var relationships = await _relationshipRepository.GetRelationshipsByTreeIdAsync(treeId);
        return relationships.Select(RelationshipMapper.ToDto);
    }

    public async Task<RelationshipDto> CreateAsync(CreateRelationshipDto dto)
    {
        // Validate that both persons exist
        var person1 = await _personRepository.GetByIdAsync(dto.Person1Id);
        if (person1 == null)
        {
            throw new NotFoundException($"Person with ID {dto.Person1Id} not found");
        }

        var person2 = await _personRepository.GetByIdAsync(dto.Person2Id);
        if (person2 == null)
        {
            throw new NotFoundException($"Person with ID {dto.Person2Id} not found");
        }

        var relationship = RelationshipMapper.ToEntity(dto);
        await _relationshipRepository.AddAsync(relationship);
        await _relationshipRepository.SaveChangesAsync();

        return RelationshipMapper.ToDto(relationship);
    }

    public async Task<RelationshipDto> UpdateAsync(Guid id, UpdateRelationshipDto dto)
    {
        var relationship = await _relationshipRepository.GetByIdAsync(id);
        if (relationship == null)
        {
            throw new NotFoundException($"Relationship with ID {id} not found");
        }

        // Validate that both persons exist
        var person1 = await _personRepository.GetByIdAsync(dto.Person1Id);
        if (person1 == null)
        {
            throw new NotFoundException($"Person with ID {dto.Person1Id} not found");
        }

        var person2 = await _personRepository.GetByIdAsync(dto.Person2Id);
        if (person2 == null)
        {
            throw new NotFoundException($"Person with ID {dto.Person2Id} not found");
        }

        RelationshipMapper.UpdateEntity(dto, relationship);
        await _relationshipRepository.UpdateAsync(relationship);
        await _relationshipRepository.SaveChangesAsync();

        return RelationshipMapper.ToDto(relationship);
    }

    public async Task DeleteAsync(Guid id)
    {
        var relationship = await _relationshipRepository.GetByIdAsync(id);
        if (relationship == null)
        {
            throw new NotFoundException($"Relationship with ID {id} not found");
        }

        await _relationshipRepository.DeleteAsync(relationship);
        await _relationshipRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<RelationshipDto>> GetPersonRelationshipsAsync(Guid personId)
    {
        var person = await _personRepository.GetByIdAsync(personId);
        if (person == null)
        {
            throw new NotFoundException($"Person with ID {personId} not found");
        }

        var relationships = await _relationshipRepository.GetPersonRelationshipsAsync(personId);
        return relationships.Select(RelationshipMapper.ToDto);
    }
}

