using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public sealed class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(150).WithMessage("Email must not exceed 150 characters.");

            RuleFor(x => x.PositionId)
                .GreaterThan(0).WithMessage("PositionId must be a positive integer.");

            RuleFor(x => x.OrganizationId)
                .GreaterThan(0).WithMessage("OrganizationId must be a positive integer.");
        }
    }

    public sealed class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be a positive integer.");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(150).WithMessage("Email must not exceed 150 characters.");

            RuleFor(x => x.PositionId)
                .GreaterThan(0).WithMessage("PositionId must be a positive integer.");

            RuleFor(x => x.OrganizationId)
                .GreaterThan(0).WithMessage("OrganizationId must be a positive integer.");
        }
    }
}
