using FluentValidation;
using WashBooking.Application.DTOs.UserProfileDTO.Request;
using WashBooking.Domain.Enums;

namespace WashBooking.Application.Validators.UserValidators;

public class UpdateUserRoleRequestValidator : AbstractValidator<UpdateUserRoleRequest>
{
    public UpdateUserRoleRequestValidator()
    {
        RuleFor(x => x.Role)
           .NotNull().WithMessage("Role is required.")
            .IsInEnum().WithMessage("Role must be one of: Admin, Staff, Customer");
    }
}