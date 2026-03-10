using FamilyTree.Application.DTOs.Person;
using FamilyTree.Domain.Entities;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Application.Mappers;

public static class PersonMapper
{
    public static PersonDto ToDto(Person entity)
    {
        return new PersonDto
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            MiddleName = entity.MiddleName,
            LastName = entity.LastName,
            MaidenName = entity.MaidenName,
            BirthDate = entity.BirthDate,
            DeathDate = entity.DeathDate,
            BirthPlace = entity.BirthPlace,
            Residence = entity.Residence,
            Gender = entity.Gender.ToString(),
            Biography = entity.Biography,
            ProfilePhotoUrl = entity.ProfilePhotoUrl,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static Person ToEntity(CreatePersonDto dto)
    {
        return new Person
        {
            FirstName = dto.FirstName,
            MiddleName = dto.MiddleName,
            LastName = dto.LastName,
            MaidenName = dto.MaidenName,
            BirthDate = dto.BirthDate,
            DeathDate = dto.DeathDate,
            BirthPlace = dto.BirthPlace,
            Residence = dto.Residence,
            Gender = Enum.Parse<Gender>(dto.Gender, ignoreCase: true),
            Biography = dto.Biography
        };
    }

    public static void UpdateEntity(UpdatePersonDto dto, Person entity)
    {
        entity.FirstName = dto.FirstName;
        entity.MiddleName = dto.MiddleName;
        entity.LastName = dto.LastName;
        entity.MaidenName = dto.MaidenName;
        entity.BirthDate = dto.BirthDate;
        entity.DeathDate = dto.DeathDate;
        entity.BirthPlace = dto.BirthPlace;
        entity.Residence = dto.Residence;
        entity.Gender = Enum.Parse<Gender>(dto.Gender, ignoreCase: true);
        entity.Biography = dto.Biography;
    }
}

