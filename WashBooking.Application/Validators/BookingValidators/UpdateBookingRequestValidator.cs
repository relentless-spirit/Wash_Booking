using FluentValidation;
using WashBooking.Application.DTOs.BookingDTO;

namespace WashBooking.Application.Validators.BookingValidators;

public class UpdateBookingRequestValidator : AbstractValidator<UpdateBookingRequest>
{
    public UpdateBookingRequestValidator()
    {
        RuleFor(x => x.BookingDateTime)
            .NotEmpty().WithMessage("Booking date time is required.");
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Items is required.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ServiceId).NotEmpty().WithMessage("Service Id is required.");
            item.RuleFor(i => i.VehicleDescription)
                .NotEmpty().WithMessage("Vehicle description is required.")
                .MaximumLength(255).WithMessage("Vehicle description cannot exceed 255 characters");
        });
    }
}