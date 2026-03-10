namespace FamilyTree.Domain.Entities;

public class PersonTree
{
    public Guid PersonId { get; set; }
    public Guid TreeId { get; set; }

    // Navigation properties
    public Person Person { get; set; } = null!;
    public Tree Tree { get; set; } = null!;
}