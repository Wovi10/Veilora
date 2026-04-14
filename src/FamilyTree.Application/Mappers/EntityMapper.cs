using FamilyTree.Application.DTOs.Entity;
using FamilyTree.Domain.Entities;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Application.Mappers;

public static class EntityMapper
{
    public static EntityDto ToDto(Entity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Type = entity.Type.ToString(),
        WorldId = entity.WorldId,
        Description = entity.Description,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        MiddleName = entity.MiddleName,
        MaidenName = entity.MaidenName,
        Species = entity.Species,
        BirthDate = entity.BirthDate,
        BirthDateSuffix = entity.BirthDateSuffix,
        DeathDate = entity.DeathDate,
        DeathDateSuffix = entity.DeathDateSuffix,
        BirthPlace = entity.BirthPlace,
        Residence = entity.Residence,
        Gender = entity.Gender?.ToString(),
        Biography = entity.Biography,
        ProfilePhotoUrl = entity.ProfilePhotoUrl,
        Parent1Id = entity.Parent1Id,
        Parent2Id = entity.Parent2Id,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    public static Entity ToEntity(CreateEntityDto dto) => new()
    {
        Name = dto.Name,
        Type = Enum.Parse<EntityType>(dto.Type),
        WorldId = dto.WorldId,
        Description = dto.Description,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        MiddleName = dto.MiddleName,
        MaidenName = dto.MaidenName,
        Species = dto.Species,
        BirthDate = dto.BirthDate,
        BirthDateSuffix = dto.BirthDateSuffix,
        DeathDate = dto.DeathDate,
        DeathDateSuffix = dto.DeathDateSuffix,
        BirthPlace = dto.BirthPlace,
        Residence = dto.Residence,
        Gender = dto.Gender is null ? null : Enum.Parse<Gender>(dto.Gender),
        Biography = dto.Biography,
        ProfilePhotoUrl = dto.ProfilePhotoUrl,
        Parent1Id = dto.Parent1Id,
        Parent2Id = dto.Parent2Id
    };

    public static void UpdateEntity(UpdateEntityDto dto, Entity entity)
    {
        entity.Name = dto.Name;
        entity.Type = Enum.Parse<EntityType>(dto.Type);
        entity.Description = dto.Description;
        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.MiddleName = dto.MiddleName;
        entity.MaidenName = dto.MaidenName;
        entity.Species = dto.Species;
        entity.BirthDate = dto.BirthDate;
        entity.BirthDateSuffix = dto.BirthDateSuffix;
        entity.DeathDate = dto.DeathDate;
        entity.DeathDateSuffix = dto.DeathDateSuffix;
        entity.BirthPlace = dto.BirthPlace;
        entity.Residence = dto.Residence;
        entity.Gender = dto.Gender is null ? null : Enum.Parse<Gender>(dto.Gender);
        entity.Biography = dto.Biography;
        entity.ProfilePhotoUrl = dto.ProfilePhotoUrl;
        entity.Parent1Id = dto.Parent1Id;
        entity.Parent2Id = dto.Parent2Id;
    }
}
