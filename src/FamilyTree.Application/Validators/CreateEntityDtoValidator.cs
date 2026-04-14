using FluentValidation;
using FamilyTree.Application.DTOs.Entity;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Application.Validators;

public class CreateEntityDtoValidator : AbstractValidator<CreateEntityDto>
{
    public CreateEntityDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Type).NotEmpty().Must(t => Enum.TryParse<EntityType>(t, out _))
            .WithMessage("Invalid entity type.");
        RuleFor(x => x.WorldId).NotEmpty();
    }
}
