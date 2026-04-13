using FluentValidation;
using FamilyTree.Application.DTOs.World;

namespace FamilyTree.Application.Validators;

public class CreateWorldDtoValidator : AbstractValidator<CreateWorldDto>
{
    public CreateWorldDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
