namespace FamilyTree.Domain.Enums;

public enum RelationshipType
{
    // Person1 is child, Person2 is parent
    ParentChildBiological,
    ParentChildAdopted,

    // Mutual relationship
    Spouse,
    Partner,

    // Person1 is godchild/ward, Person2 is godparent/guardian
    Godparent,
    Guardian,

    // Mutual relationship
    CloseFriend
}