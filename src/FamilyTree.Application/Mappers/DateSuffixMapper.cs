using FamilyTree.Application.DTOs.DateSuffix;
using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Mappers;

public static class DateSuffixMapper
{
    public static DateSuffixDto ToDto(DateSuffix d) =>
        new(d.Id, d.Name, d.Abbreviation, d.AnchorYear, d.Scale, d.IsReversed, d.IsDefault, d.WorldId);
}
