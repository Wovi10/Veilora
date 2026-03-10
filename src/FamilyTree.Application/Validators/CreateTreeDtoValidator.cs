using FluentValidation;
using FamilyTree.Application.DTOs.Tree;

namespace FamilyTree.Application.Validators;

public class CreateTreeDtoValidator : AbstractValidator<CreateTreeDto>
{
    public CreateTreeDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tree name is required")
            .MaximumLength(200).WithMessage("Tree name must not exceed 200 characters");
    }
}

