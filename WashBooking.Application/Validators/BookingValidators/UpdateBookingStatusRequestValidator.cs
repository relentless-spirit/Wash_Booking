using FluentValidation;
using WashBooking.Application.DTOs.BookingDTO.Request;
using WashBooking.Domain.Enums;

namespace WashBooking.Application.Validators.BookingValidators;

public class UpdateBookingStatusRequestValidator : AbstractValidator<UpdateBookingStatusRequest>
{
    public UpdateBookingStatusRequestValidator()
    {
        RuleFor(x => x.NewStatus)
            .NotNull().WithMessage("NewStatus is required.")
            .IsInEnum().WithMessage($"NewStatus must be a valid BookingStatus enum value. Examples: {string.Join(", ", System.Enum.GetNames(typeof(BookingStatus)))}");
    }
}