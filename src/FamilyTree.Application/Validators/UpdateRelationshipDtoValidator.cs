using FluentValidation;
using FamilyTree.Application.DTOs.Relationship;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Application.Validators;

public class UpdateRelationshipDtoValidator : AbstractValidator<UpdateRelationshipDto>
{
    public UpdateRelationshipDtoValidator()
    {
        RuleFor(x => x.Character1Id).NotEmpty();
        RuleFor(x => x.Character2Id).NotEmpty()
            .NotEqual(x => x.Character1Id).WithMessage("Character2Id must differ from Character1Id.");
        RuleFor(x => x.RelationshipType).NotEmpty()
            .Must(t => Enum.TryParse<RelationshipType>(t, out _))
            .WithMessage("Invalid relationship type.");
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}
