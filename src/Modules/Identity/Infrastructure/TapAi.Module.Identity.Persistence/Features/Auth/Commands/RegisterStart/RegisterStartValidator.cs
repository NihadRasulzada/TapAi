using FluentValidation;

namespace TapAi.Module.Identity.Persistence.Features.Auth.Commands.RegisterStart;

public sealed class RegisterStartValidator : AbstractValidator<RegisterStartRequest>
{
    public RegisterStartValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[0-9]{7,15}$")
            .WithMessage("Phone number must be 7-15 digits, optionally starting with '+'.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.");
    }
}
