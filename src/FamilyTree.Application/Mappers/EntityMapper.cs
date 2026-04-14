using FamilyTree.Application.DTOs.Entity;
using FamilyTree.Application.DTOs.Language;
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
        BirthPlaceEntityId = entity.BirthPlaceEntityId,
        BirthPlaceEntityName = entity.BirthPlaceEntity?.Name,
        DeathPlaceEntityId = entity.DeathPlaceEntityId,
        DeathPlaceEntityName = entity.DeathPlaceEntity?.Name,
        Residence = entity.Residence,
        Gender = entity.Gender?.ToString(),
        Biography = entity.Biography,
        ProfilePhotoUrl = entity.ProfilePhotoUrl,
        OtherNames = entity.OtherNames,
        Position = entity.Position,
        Height = entity.Height,
        HairColour = entity.HairColour,
        Parent1Id = entity.Parent1Id,
        Parent2Id = entity.Parent2Id,
        Locations = entity.Locations
            .Select(l => new EntityRefDto(l.PlaceId, l.Place?.Name ?? string.Empty)).ToList(),
        Affiliations = entity.Affiliations
            .Select(a => new EntityRefDto(a.GroupId, a.Group?.Name ?? string.Empty)).ToList(),
        Languages = entity.Languages
            .Select(l => new LanguageDto(l.LanguageId, l.Language?.Name ?? string.Empty, l.Language?.WorldId ?? entity.WorldId)).ToList(),
        // Spouses and Children populated by service via EnrichWithRelationalDataAsync
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
        entity.BirthPlaceEntityId = dto.BirthPlaceEntityId;
        entity.DeathPlaceEntityId = dto.DeathPlaceEntityId;
        entity.Residence = dto.Residence;
        entity.Gender = dto.Gender is null ? null : Enum.Parse<Gender>(dto.Gender);
        entity.Biography = dto.Biography;
        entity.ProfilePhotoUrl = dto.ProfilePhotoUrl;
        entity.OtherNames = dto.OtherNames;
        entity.Position = dto.Position;
        entity.Height = dto.Height;
        entity.HairColour = dto.HairColour;
        entity.Parent1Id = dto.Parent1Id;
        entity.Parent2Id = dto.Parent2Id;
    }
}
