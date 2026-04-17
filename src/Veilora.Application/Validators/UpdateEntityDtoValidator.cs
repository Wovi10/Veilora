using FluentValidation;
using Veilora.Application.DTOs.Entity;
using Veilora.Domain.Enums;

namespace Veilora.Application.Validators;

public class UpdateEntityDtoValidator : AbstractValidator<UpdateEntityDto>
{
    public UpdateEntityDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Type).NotEmpty().Must(t => Enum.TryParse<EntityType>(t, out _))
            .WithMessage("Invalid entity type.");
    }
}
