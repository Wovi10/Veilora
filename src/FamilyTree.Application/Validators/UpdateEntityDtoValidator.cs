using FluentValidation;
using FamilyTree.Application.DTOs.Entity;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Application.Validators;

public class UpdateEntityDtoValidator : AbstractValidator<UpdateEntityDto>
{
    public UpdateEntityDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Type).NotEmpty().Must(t => Enum.TryParse<EntityType>(t, out _))
            .WithMessage("Invalid entity type.");
        RuleFor(x => x.FirstName).MaximumLength(100).When(x => x.FirstName is not null);
        RuleFor(x => x.LastName).MaximumLength(100).When(x => x.LastName is not null);
        RuleFor(x => x.Gender).Must(g => Enum.TryParse<Gender>(g, out _))
            .When(x => x.Gender is not null).WithMessage("Invalid gender value.");
        RuleFor(x => x.DeathDate).GreaterThan(x => x.BirthDate)
            .When(x => x.BirthDate.HasValue && x.DeathDate.HasValue);
    }
}
