using FamilyTree.Application.DTOs.Relationship;
using FamilyTree.Domain.Entities;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Application.Mappers;

public static class RelationshipMapper
{
    public static RelationshipDto ToDto(Relationship relationship) => new()
    {
        Id = relationship.Id,
        Entity1Id = relationship.Entity1Id,
        Entity2Id = relationship.Entity2Id,
        RelationshipType = relationship.RelationshipType.ToString(),
        StartDate = relationship.StartDate,
        EndDate = relationship.EndDate,
        Notes = relationship.Notes,
        CreatedAt = relationship.CreatedAt,
        UpdatedAt = relationship.UpdatedAt
    };

    public static Relationship ToEntity(CreateRelationshipDto dto) => new()
    {
        Entity1Id = dto.Entity1Id,
        Entity2Id = dto.Entity2Id,
        RelationshipType = Enum.Parse<RelationshipType>(dto.RelationshipType),
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        Notes = dto.Notes
    };

    public static void UpdateEntity(UpdateRelationshipDto dto, Relationship relationship)
    {
        relationship.Entity1Id = dto.Entity1Id;
        relationship.Entity2Id = dto.Entity2Id;
        relationship.RelationshipType = Enum.Parse<RelationshipType>(dto.RelationshipType);
        relationship.StartDate = dto.StartDate;
        relationship.EndDate = dto.EndDate;
        relationship.Notes = dto.Notes;
    }
}
