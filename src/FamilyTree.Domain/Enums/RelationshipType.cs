namespace FamilyTree.Domain.Enums;

public enum RelationshipType
{
    // Mutual relationships
    Spouse,
    Partner,

    // Person1 is godchild/ward, Person2 is godparent/guardian
    Godparent,
    Guardian,

    // Mutual relationship
    CloseFriend
}