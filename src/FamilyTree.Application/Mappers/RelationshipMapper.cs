using FamilyTree.Application.DTOs.Relationship;
using FamilyTree.Domain.Entities;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Application.Mappers;

public static class RelationshipMapper
{
    public static RelationshipDto ToDto(Relationship entity)
    {
        return new RelationshipDto
        {
            Id = entity.Id,
            Person1Id = entity.Person1Id,
            Person2Id = entity.Person2Id,
            RelationshipType = entity.RelationshipType.ToString(),
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static Relationship ToEntity(CreateRelationshipDto dto)
    {
        return new Relationship
        {
            Person1Id = dto.Person1Id,
            Person2Id = dto.Person2Id,
            RelationshipType = Enum.Parse<RelationshipType>(dto.RelationshipType, ignoreCase: true),
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Notes = dto.Notes
        };
    }

    public static void UpdateEntity(UpdateRelationshipDto dto, Relationship entity)
    {
        entity.Person1Id = dto.Person1Id;
        entity.Person2Id = dto.Person2Id;
        entity.RelationshipType = Enum.Parse<RelationshipType>(dto.RelationshipType, ignoreCase: true);
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.Notes = dto.Notes;
    }
}

