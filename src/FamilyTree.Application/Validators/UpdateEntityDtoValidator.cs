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
    }
}
