using FluentValidation;
using FamilyTree.Application.DTOs.Person;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Application.Validators;

public class CreatePersonDtoValidator : AbstractValidator<CreatePersonDto>
{
    public CreatePersonDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

        RuleFor(x => x.MiddleName)
            .MaximumLength(100).WithMessage("Middle name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.MiddleName));

        RuleFor(x => x.MaidenName)
            .MaximumLength(100).WithMessage("Maiden name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.MaidenName));

        RuleFor(x => x.BirthPlace)
            .MaximumLength(200).WithMessage("Birth place must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.BirthPlace));

        RuleFor(x => x.Residence)
            .MaximumLength(200).WithMessage("Residence must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Residence));

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required")
            .Must(BeValidGender).WithMessage("Gender must be a valid value (Male, Female, Other, PreferNotToSay)");

        RuleFor(x => x.DeathDate)
            .GreaterThan(x => x.BirthDate).WithMessage("Death date must be after birth date")
            .When(x => x.BirthDate.HasValue && x.DeathDate.HasValue);
    }

    private bool BeValidGender(string gender)
    {
        return Enum.TryParse<Gender>(gender, ignoreCase: true, out _);
    }
}

