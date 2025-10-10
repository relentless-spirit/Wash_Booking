using FluentValidation;
using WashBooking.Application.DTOs.ServiceDTO;

namespace WashBooking.Application.Validators.ServiceValidators;

public class GetPagedRequestValidator : AbstractValidator<GetPagedRequest>
{
    public GetPagedRequestValidator()
    {
        RuleFor(x => x.PageIndex)
            .NotEmpty().WithMessage("Page index is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Page index must be greater than or equal to 0.");
        RuleFor(x => x.PageSize)
            .NotEmpty().WithMessage("Page size is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Page size must be greater than or equal to 0.");
    }
}