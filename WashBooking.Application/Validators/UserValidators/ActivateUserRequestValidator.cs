using FluentValidation;
using WashBooking.Application.DTOs.UserProfileDTO.Request;

namespace WashBooking.Application.Validators.UserValidators;

public class ActivateUserRequestValidator : AbstractValidator<ActivateUserRequest>
{
    public ActivateUserRequestValidator()
    {
        RuleFor(x => x.IsActive)
            .NotEmpty().WithMessage("Is active is required.");
    }
}