using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public sealed class PositionCreateDtoValidator : AbstractValidator<PositionCreateDto>
    {
        public PositionCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .When(x => x.Description != null);
        }
    }

    public sealed class PositionUpdateDtoValidator : AbstractValidator<PositionUpdateDto>
    {
        public PositionUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be a positive integer.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .When(x => x.Description != null);
        }
    }
}
