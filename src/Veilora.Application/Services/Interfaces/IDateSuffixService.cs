using Veilora.Application.DTOs.DateSuffix;

namespace Veilora.Application.Services.Interfaces;

public interface IDateSuffixService
{
    Task<IEnumerable<DateSuffixDto>> GetByWorldIdAsync(Guid worldId);
    Task<DateSuffixDto> CreateAsync(CreateDateSuffixDto dto);
    Task<DateSuffixDto> UpdateAsync(Guid id, UpdateDateSuffixDto dto);
    Task DeleteAsync(Guid id);
}
