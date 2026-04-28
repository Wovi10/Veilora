using Veilora.Domain.Common;

namespace Veilora.Domain.Entities;

public class ReadingNote : BaseEntity
{
    public required Guid SessionId { get; set; }
    public required Guid WorldId { get; set; }
    public required Guid UserId { get; set; }
    public required string Text { get; set; }
    public string[] Tags { get; set; } = [];

    public ReadingSession? Session { get; set; }
}
