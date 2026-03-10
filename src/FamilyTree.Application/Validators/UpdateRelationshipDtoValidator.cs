using FluentValidation;
using FamilyTree.Application.DTOs.Relationship;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Application.Validators;

public class UpdateRelationshipDtoValidator : AbstractValidator<UpdateRelationshipDto>
{
    public UpdateRelationshipDtoValidator()
    {
        RuleFor(x => x.Person1Id)
            .NotEmpty().WithMessage("Person1Id is required");

        RuleFor(x => x.Person2Id)
            .NotEmpty().WithMessage("Person2Id is required");

        RuleFor(x => x)
            .Must(x => x.Person1Id != x.Person2Id)
            .WithMessage("Person1Id and Person2Id must be different")
            .When(x => x.Person1Id != Guid.Empty && x.Person2Id != Guid.Empty);

        RuleFor(x => x.RelationshipType)
            .NotEmpty().WithMessage("Relationship type is required")
            .Must(BeValidRelationshipType).WithMessage("Relationship type must be a valid value");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }

    private bool BeValidRelationshipType(string relationshipType)
    {
        return Enum.TryParse<RelationshipType>(relationshipType, ignoreCase: true, out _);
    }
}

