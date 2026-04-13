using FamilyTree.Application.DTOs.FamilyTree;
using FamilyTreeEntity = FamilyTree.Domain.Entities.FamilyTree;

namespace FamilyTree.Application.Mappers;

public static class FamilyTreeMapper
{
    public static FamilyTreeDto ToDto(FamilyTreeEntity familyTree) => new()
    {
        Id = familyTree.Id,
        Name = familyTree.Name,
        Description = familyTree.Description,
        WorldId = familyTree.WorldId,
        CreatedAt = familyTree.CreatedAt,
        UpdatedAt = familyTree.UpdatedAt
    };

    public static FamilyTreeEntity ToEntity(CreateFamilyTreeDto dto) => new()
    {
        Name = dto.Name,
        Description = dto.Description,
        WorldId = dto.WorldId
    };

    public static void UpdateEntity(UpdateFamilyTreeDto dto, FamilyTreeEntity familyTree)
    {
        familyTree.Name = dto.Name;
        familyTree.Description = dto.Description;
    }

    public static FamilyTreeWithEntitiesDto ToWithEntitiesDto(FamilyTreeEntity familyTree) => new()
    {
        Id = familyTree.Id,
        Name = familyTree.Name,
        Description = familyTree.Description,
        WorldId = familyTree.WorldId,
        Entities = familyTree.EntityFamilyTrees.Select(eft => new EntityInFamilyTreeDto
        {
            Entity = EntityMapper.ToDto(eft.Entity),
            PositionX = eft.PositionX,
            PositionY = eft.PositionY
        }),
        CreatedAt = familyTree.CreatedAt,
        UpdatedAt = familyTree.UpdatedAt
    };
}
