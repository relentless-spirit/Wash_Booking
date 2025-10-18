using FluentValidation;
using WashBooking.Application.DTOs.ServiceDTO.BookingDetailDTO.Request;

namespace WashBooking.Application.Validators.BookingDetailValidators;

public class AssignStaffRequestValidator : AbstractValidator<AssignStaffRequest>
{
    public AssignStaffRequestValidator()
    {
        RuleFor(x => x.NewAssigneeId)
            .NotEmpty().WithMessage("New assignee id is required.");
    }
}