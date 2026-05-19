using Veilora.Domain.Common;
using Veilora.Domain.Enums;

namespace Veilora.Domain.Entities;

public class Photo : BaseEntity
{
    public Guid EntityId { get; set; }
    public PhotoTargetType EntityType { get; set; }
    public required string Url { get; set; }
    public int DisplayOrder { get; set; }
}
