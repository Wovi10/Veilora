using FamilyTree.Application.DTOs.DateSuffix;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Mappers;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services.Interfaces;
using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Services;

public class DateSuffixService(
    IDateSuffixRepository dateSuffixRepository,
    IWorldRepository worldRepository) : IDateSuffixService
{
    public async Task<IEnumerable<DateSuffixDto>> GetByWorldIdAsync(Guid worldId)
    {
        var suffixes = await dateSuffixRepository.GetByWorldIdAsync(worldId);
        return suffixes.Select(DateSuffixMapper.ToDto);
    }

    public async Task<DateSuffixDto> CreateAsync(CreateDateSuffixDto dto)
    {
        _ = await worldRepository.GetByIdAsync(dto.WorldId)
            ?? throw new NotFoundException(nameof(World), dto.WorldId);

        if (dto.IsDefault)
            await dateSuffixRepository.ClearDefaultForWorldAsync(dto.WorldId);

        var suffix = new DateSuffix
        {
            Name = dto.Name.Trim(),
            Abbreviation = dto.Abbreviation.Trim(),
            AnchorYear = dto.AnchorYear,
            Scale = dto.Scale,
            IsReversed = dto.IsReversed,
            IsDefault = dto.IsDefault,
            WorldId = dto.WorldId
        };

        await dateSuffixRepository.AddAsync(suffix);
        await dateSuffixRepository.SaveChangesAsync();
        return DateSuffixMapper.ToDto(suffix);
    }

    public async Task<DateSuffixDto> UpdateAsync(Guid id, UpdateDateSuffixDto dto)
    {
        var suffix = await dateSuffixRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(DateSuffix), id);

        if (dto.IsDefault)
            await dateSuffixRepository.ClearDefaultForWorldAsync(suffix.WorldId);

        suffix.Name = dto.Name.Trim();
        suffix.Abbreviation = dto.Abbreviation.Trim();
        suffix.AnchorYear = dto.AnchorYear;
        suffix.Scale = dto.Scale;
        suffix.IsReversed = dto.IsReversed;
        suffix.IsDefault = dto.IsDefault;

        await dateSuffixRepository.UpdateAsync(suffix);
        await dateSuffixRepository.SaveChangesAsync();
        return DateSuffixMapper.ToDto(suffix);
    }

    public async Task DeleteAsync(Guid id)
    {
        var suffix = await dateSuffixRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(DateSuffix), id);
        await dateSuffixRepository.DeleteAsync(suffix);
        await dateSuffixRepository.SaveChangesAsync();
    }
}
