using Veilora.Domain.Common;

namespace Veilora.Domain.Entities;

public class ReadingSession : BaseEntity
{
    public required Guid WorldId { get; set; }
    public required Guid UserId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public bool IsActive => EndedAt is null;

    public World? World { get; set; }
    public ICollection<ReadingNote> Notes { get; set; } = [];
}
