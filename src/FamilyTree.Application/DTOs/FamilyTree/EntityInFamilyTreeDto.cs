using FamilyTree.Application.DTOs.Entity;

namespace FamilyTree.Application.DTOs.FamilyTree;

public record EntityInFamilyTreeDto
{
    public required EntityDto Entity { get; init; }
    public double? PositionX { get; init; }
    public double? PositionY { get; init; }
}
