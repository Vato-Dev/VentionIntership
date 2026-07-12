using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public sealed class OrganizationCreateDtoValidator : AbstractValidator<OrganizationCreateDto>
    {
        public OrganizationCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Organization name is required.")
                .MaximumLength(150).WithMessage("Organization name must not exceed 150 characters.");

            RuleFor(x => x.StreetAddress)
                .NotEmpty().WithMessage("Street address is required.")
                .MaximumLength(250).WithMessage("Street address must not exceed 250 characters.");
        }
    }

    public sealed class OrganizationUpdateDtoValidator : AbstractValidator<OrganizationUpdateDto>
    {
        public OrganizationUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be a positive integer.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Organization name is required.")
                .MaximumLength(150).WithMessage("Organization name must not exceed 150 characters.");

            RuleFor(x => x.StreetAddress)
                .NotEmpty().WithMessage("Street address is required.")
                .MaximumLength(250).WithMessage("Street address must not exceed 250 characters.");
        }
    }
}
