using FluentValidation;
using Veilora.Application.DTOs.FamilyTree;

namespace Veilora.Application.Validators;

public class CreateFamilyTreeDtoValidator : AbstractValidator<CreateFamilyTreeDto>
{
    public CreateFamilyTreeDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.WorldId).NotEmpty();
    }
}
