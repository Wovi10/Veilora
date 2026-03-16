namespace FamilyTree.Application.DTOs.Tree;

public record TreeWithPersonsDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public List<PersonInTreeDto> Persons { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}