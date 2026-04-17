using Veilora.Application.DTOs.Relationship;
using Veilora.Domain.Entities;
using Veilora.Domain.Enums;

namespace Veilora.Application.Mappers;

public static class RelationshipMapper
{
    public static RelationshipDto ToDto(Relationship relationship) => new()
    {
        Id = relationship.Id,
        Character1Id = relationship.Character1Id,
        Character2Id = relationship.Character2Id,
        RelationshipType = relationship.RelationshipType.ToString(),
        StartDate = relationship.StartDate,
        EndDate = relationship.EndDate,
        Notes = relationship.Notes,
        CreatedAt = relationship.CreatedAt,
        UpdatedAt = relationship.UpdatedAt
    };

    public static Relationship ToEntity(CreateRelationshipDto dto) => new()
    {
        Character1Id = dto.Character1Id,
        Character2Id = dto.Character2Id,
        RelationshipType = Enum.Parse<RelationshipType>(dto.RelationshipType),
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        Notes = dto.Notes
    };

    public static void UpdateEntity(UpdateRelationshipDto dto, Relationship relationship)
    {
        relationship.Character1Id = dto.Character1Id;
        relationship.Character2Id = dto.Character2Id;
        relationship.RelationshipType = Enum.Parse<RelationshipType>(dto.RelationshipType);
        relationship.StartDate = dto.StartDate;
        relationship.EndDate = dto.EndDate;
        relationship.Notes = dto.Notes;
    }
}
