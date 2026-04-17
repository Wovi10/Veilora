using Veilora.Application.DTOs.FamilyTree;
using FamilyTreeEntity = Veilora.Domain.Entities.FamilyTree;

namespace Veilora.Application.Mappers;

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
        Characters = familyTree.CharacterFamilyTrees.Select(cft => new CharacterInFamilyTreeDto
        {
            Character = CharacterMapper.ToDto(cft.Character),
            PositionX = cft.PositionX,
            PositionY = cft.PositionY
        }),
        CreatedAt = familyTree.CreatedAt,
        UpdatedAt = familyTree.UpdatedAt
    };
}
