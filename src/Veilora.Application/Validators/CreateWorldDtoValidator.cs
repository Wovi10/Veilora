using FluentValidation;
using Veilora.Application.DTOs.World;

namespace Veilora.Application.Validators;

public class CreateWorldDtoValidator : AbstractValidator<CreateWorldDto>
{
    public CreateWorldDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
