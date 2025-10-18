using FluentValidation;
using WashBooking.Application.DTOs.ServiceDTO.BookingDetailDTO.Request;

namespace WashBooking.Application.Validators.BookingDetailValidators;

public class UpdateBookingDetailStatusRequestValidator : AbstractValidator<UpdateBookingDetailStatusRequest>
{
    public UpdateBookingDetailStatusRequestValidator()
    {
        RuleFor(x => x.NewStatus)
            .NotEmpty().WithMessage("New status is required.")
            .IsInEnum().WithMessage("New status is not valid.");
    }
}