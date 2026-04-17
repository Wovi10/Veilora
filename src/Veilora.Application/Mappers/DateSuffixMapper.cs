using Veilora.Application.DTOs.DateSuffix;
using Veilora.Domain.Entities;

namespace Veilora.Application.Mappers;

public static class DateSuffixMapper
{
    public static DateSuffixDto ToDto(DateSuffix d) =>
        new(d.Id, d.Name, d.Abbreviation, d.AnchorYear, d.Scale, d.IsReversed, d.IsDefault, d.WorldId);
}
