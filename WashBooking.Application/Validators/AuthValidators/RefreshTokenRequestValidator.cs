using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WashBooking.Application.DTOs.AuthDTO.RefreshTokenDTO;

namespace WashBooking.Application.Validators.AuthValidators
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.")
                .Must(token => !string.IsNullOrWhiteSpace(token)).WithMessage("Refresh token cannot be whitespace.");
        }
    }
}
