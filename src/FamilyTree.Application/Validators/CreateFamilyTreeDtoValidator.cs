using FluentValidation;
using FamilyTree.Application.DTOs.FamilyTree;

namespace FamilyTree.Application.Validators;

public class CreateFamilyTreeDtoValidator : AbstractValidator<CreateFamilyTreeDto>
{
    public CreateFamilyTreeDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.WorldId).NotEmpty();
    }
}
