using FamilyTree.Application.DTOs.Character;
using FamilyTree.Application.DTOs.Entity;
using FamilyTree.Application.DTOs.Language;
using FamilyTree.Domain.Entities;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Application.Mappers;

public static class CharacterMapper
{
    public static CharacterDto ToDto(Character character) => new()
    {
        Id = character.Id,
        Name = character.Name,
        WorldId = character.WorldId,
        Description = character.Description,
        FirstName = character.FirstName,
        LastName = character.LastName,
        MiddleName = character.MiddleName,
        MaidenName = character.MaidenName,
        Species = character.Species,
        Gender = character.Gender?.ToString(),
        BirthDate = character.BirthDate,
        BirthDateSuffix = character.BirthDateSuffix,
        DeathDate = character.DeathDate,
        DeathDateSuffix = character.DeathDateSuffix,
        BirthPlaceEntityId = character.BirthPlaceEntityId,
        BirthPlaceEntityName = character.BirthPlaceEntity?.Name,
        DeathPlaceEntityId = character.DeathPlaceEntityId,
        DeathPlaceEntityName = character.DeathPlaceEntity?.Name,
        Residence = character.Residence,
        Biography = character.Biography,
        ProfilePhotoUrl = character.ProfilePhotoUrl,
        OtherNames = character.OtherNames,
        Position = character.Position,
        Height = character.Height,
        HairColour = character.HairColour,
        Parent1Id = character.Parent1Id,
        Parent2Id = character.Parent2Id,
        Locations = character.Locations
            .Select(l => new EntityRefDto(l.PlaceId, l.Place?.Name ?? string.Empty)).ToList(),
        Affiliations = character.Affiliations
            .Select(a => new EntityRefDto(a.GroupId, a.Group?.Name ?? string.Empty)).ToList(),
        Languages = character.Languages
            .Select(l => new LanguageDto(l.LanguageId, l.Language?.Name ?? string.Empty, l.Language?.WorldId ?? character.WorldId)).ToList(),
        // Spouses and Children populated by service via EnrichWithRelationalDataAsync
        CreatedAt = character.CreatedAt,
        UpdatedAt = character.UpdatedAt
    };

    public static Character ToEntity(CreateCharacterDto dto) => new()
    {
        Name = dto.Name,
        WorldId = dto.WorldId,
        Description = dto.Description,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        MiddleName = dto.MiddleName,
        MaidenName = dto.MaidenName,
        Species = dto.Species,
        Gender = dto.Gender is null ? null : Enum.Parse<Gender>(dto.Gender),
        BirthDate = dto.BirthDate,
        BirthDateSuffix = dto.BirthDateSuffix,
        DeathDate = dto.DeathDate,
        DeathDateSuffix = dto.DeathDateSuffix,
        BirthPlaceEntityId = dto.BirthPlaceEntityId,
        DeathPlaceEntityId = dto.DeathPlaceEntityId,
        Residence = dto.Residence,
        Biography = dto.Biography,
        ProfilePhotoUrl = dto.ProfilePhotoUrl,
        OtherNames = dto.OtherNames,
        Position = dto.Position,
        Height = dto.Height,
        HairColour = dto.HairColour,
        Parent1Id = dto.Parent1Id,
        Parent2Id = dto.Parent2Id
    };

    public static void UpdateCharacter(UpdateCharacterDto dto, Character character)
    {
        character.Name = dto.Name;
        character.Description = dto.Description;
        character.FirstName = dto.FirstName;
        character.LastName = dto.LastName;
        character.MiddleName = dto.MiddleName;
        character.MaidenName = dto.MaidenName;
        character.Species = dto.Species;
        character.Gender = dto.Gender is null ? null : Enum.Parse<Gender>(dto.Gender);
        character.BirthDate = dto.BirthDate;
        character.BirthDateSuffix = dto.BirthDateSuffix;
        character.DeathDate = dto.DeathDate;
        character.DeathDateSuffix = dto.DeathDateSuffix;
        // BirthPlaceEntityId and DeathPlaceEntityId are resolved and set by the service
        character.Residence = dto.Residence;
        character.Biography = dto.Biography;
        character.ProfilePhotoUrl = dto.ProfilePhotoUrl;
        character.OtherNames = dto.OtherNames;
        character.Position = dto.Position;
        character.Height = dto.Height;
        character.HairColour = dto.HairColour;
        character.Parent1Id = dto.Parent1Id;
        character.Parent2Id = dto.Parent2Id;
    }
}
