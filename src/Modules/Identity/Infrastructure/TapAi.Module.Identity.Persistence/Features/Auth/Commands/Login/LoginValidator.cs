using FluentValidation;

namespace TapAi.Module.Identity.Persistence.Features.Auth.Commands.Login;

public sealed class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[0-9]{7,15}$")
            .WithMessage("A valid phone number is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(128);
    }
}
