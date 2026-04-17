using Veilora.Domain.Common;

namespace Veilora.Domain.Entities;

public class Language : BaseEntity
{
    public required string Name { get; set; }
    public Guid WorldId { get; set; }

    public World World { get; set; } = null!;
    public ICollection<EntityLanguage> EntityLanguages { get; set; } = [];
}
