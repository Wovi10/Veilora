namespace FamilyTree.Application.DTOs.DateSuffix;

public record DateSuffixDto(Guid Id, string Name, string Abbreviation, int Order, bool IsDefault, Guid WorldId);
