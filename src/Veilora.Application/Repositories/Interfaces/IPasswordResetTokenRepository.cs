using Veilora.Domain.Entities;

namespace Veilora.Application.Repositories.Interfaces;

public interface IPasswordResetTokenRepository : IRepository<PasswordResetToken>
{
    Task<PasswordResetToken?> GetValidByTokenHashAsync(string tokenHash);
    Task InvalidateAllForUserAsync(Guid userId);
}
