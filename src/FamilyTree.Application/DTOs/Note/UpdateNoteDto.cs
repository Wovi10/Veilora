namespace FamilyTree.Application.DTOs.Note;

public record UpdateNoteDto
{
    public required string Content { get; init; }
    public Guid? WorldId { get; init; }
    public Guid? EntityId { get; init; }
}
