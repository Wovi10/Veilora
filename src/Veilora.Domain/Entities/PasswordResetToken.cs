using Veilora.Domain.Common;

namespace Veilora.Domain.Entities;

public class PasswordResetToken : BaseEntity
{
    public required Guid UserId { get; set; }
    public User? User { get; set; }
    public required string TokenHash { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
}
