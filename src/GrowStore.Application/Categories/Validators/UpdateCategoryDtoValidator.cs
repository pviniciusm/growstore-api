using FluentValidation;
using GrowStore.Application.Categories.DTOs;

namespace GrowStore.Application.Categories.Validators;

public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name must be at most 100 characters long.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must be at most 500 characters long.")
            .When(x => x.Description is not null);
    }
}
