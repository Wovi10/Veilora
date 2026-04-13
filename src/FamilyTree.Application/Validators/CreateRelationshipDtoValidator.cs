using FluentValidation;
using FamilyTree.Application.DTOs.Relationship;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Application.Validators;

public class CreateRelationshipDtoValidator : AbstractValidator<CreateRelationshipDto>
{
    public CreateRelationshipDtoValidator()
    {
        RuleFor(x => x.Entity1Id).NotEmpty();
        RuleFor(x => x.Entity2Id).NotEmpty()
            .NotEqual(x => x.Entity1Id).WithMessage("Entity2Id must differ from Entity1Id.");
        RuleFor(x => x.RelationshipType).NotEmpty()
            .Must(t => Enum.TryParse<RelationshipType>(t, out _))
            .WithMessage("Invalid relationship type.");
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}
