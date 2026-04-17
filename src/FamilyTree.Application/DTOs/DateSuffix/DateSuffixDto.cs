namespace FamilyTree.Application.DTOs.DateSuffix;

public record DateSuffixDto(Guid Id, string Name, string Abbreviation, long AnchorYear, decimal Scale, bool IsReversed, bool IsDefault, Guid WorldId);
