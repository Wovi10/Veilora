using Veilora.Application.Repositories.Interfaces;
using Veilora.Domain.Entities;
using Veilora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Veilora.Infrastructure.Repositories;

public class PasswordResetTokenRepository(ApplicationDbContext context)
    : Repository<PasswordResetToken>(context), IPasswordResetTokenRepository
{
    public async Task<PasswordResetToken?> GetValidByTokenHashAsync(string tokenHash)
    {
        var now = DateTime.UtcNow;
        return await _context.PasswordResetTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && t.UsedAt == null && t.ExpiresAt > now);
    }

    public async Task InvalidateAllForUserAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        await _context.PasswordResetTokens
            .Where(t => t.UserId == userId && t.UsedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.UsedAt, now));
    }
}
