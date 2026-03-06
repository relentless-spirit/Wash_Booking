using FluentValidation;

namespace Modules.Users.Application.Authentications.Commands.RegisterUser;

internal sealed class RegisterUserCommandValidation : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidation()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .Length(2, 50).WithMessage("First name must be between 2 and 50 characters.")
            .OverridePropertyName("User.Register.FirstName");
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .Length(2, 50)
            .WithMessage("Last name must be between 2 and 50 characters.")
            .OverridePropertyName("User.Register.LastName");
        RuleFor(x => x.Username)

            .NotEmpty()
            .WithMessage("Username is required.")
            .Length(5, 50)
            .WithMessage("Username must be between 5 and 50 characters.")
            .OverridePropertyName("User.Register.Username");
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.")
            .MaximumLength(100)
            .WithMessage("Email must not exceed 100 characters.")
            .OverridePropertyName("User.Register.Email");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("Password must contain at least one special character.")
            .OverridePropertyName("User.Register.Password");
    }
}