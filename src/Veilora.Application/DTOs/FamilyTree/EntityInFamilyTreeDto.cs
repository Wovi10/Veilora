using Veilora.Application.DTOs.Character;

namespace Veilora.Application.DTOs.FamilyTree;

public record CharacterInFamilyTreeDto
{
    public required CharacterDto Character { get; init; }
    public double? PositionX { get; init; }
    public double? PositionY { get; init; }
}
