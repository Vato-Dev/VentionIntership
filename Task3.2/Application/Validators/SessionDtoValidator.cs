using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public sealed class SessionCreateDtoValidator : AbstractValidator<SessionCreateDto>
    {
        public SessionCreateDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("User ID must be a positive integer.");

            RuleFor(x => x.ExpiresAt)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Expiration time must be in the future.");
        }
    }
    public sealed class SessionUpdateDtoValidator : AbstractValidator<SessionUpdateDto>
    {
        public SessionUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Session ID must be a positive integer.");

            RuleFor(x => x.ExpiresAt)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("New expiration time must be in the future.");
            
        }
    }
}
