namespace Veilora.Domain.Entities;

public class EntityLanguage
{
    public Guid CharacterId { get; set; }
    public Guid LanguageId { get; set; }

    public Character Character { get; set; } = null!;
    public Language Language { get; set; } = null!;
}
