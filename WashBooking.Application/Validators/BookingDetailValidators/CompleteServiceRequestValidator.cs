using FluentValidation;
using WashBooking.Application.DTOs.ServiceDTO.BookingDetailDTO.Request;

namespace WashBooking.Application.Validators.BookingDetailValidators;

public class CompleteServiceRequestValidator : AbstractValidator<CompleteServiceRequest>
{
    public CompleteServiceRequestValidator()
    {
        RuleFor(x => x.Note)
            .NotEmpty().WithMessage("Note is required.")
            .MaximumLength(255).WithMessage("Note cannot exceed 255 characters.");
    }
}