using FamilyTree.Application.DTOs.Tree;
using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Mappers;

public static class TreeMapper
{
    public static TreeDto ToDto(Tree entity)
    {
        return new TreeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static Tree ToEntity(CreateTreeDto dto)
    {
        return new Tree
        {
            Name = dto.Name,
            Description = dto.Description
        };
    }

    public static void UpdateEntity(UpdateTreeDto dto, Tree entity)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
    }

    public static TreeWithPersonsDto ToTreeWithPersonsDto(Tree entity)
    {
        return new TreeWithPersonsDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Persons = entity.PersonTrees.Select(pt => new PersonInTreeDto
            {
                Person = PersonMapper.ToDto(pt.Person),
                PositionX = pt.PositionX,
                PositionY = pt.PositionY
            }).ToList(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}


