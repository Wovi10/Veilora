using FamilyTree.Application.DTOs.Person;

namespace FamilyTree.Application.DTOs.Tree;

public record PersonInTreeDto
{
    public PersonDto Person { get; init; } = null!;
    public double? PositionX { get; init; }
    public double? PositionY { get; init; }
}
