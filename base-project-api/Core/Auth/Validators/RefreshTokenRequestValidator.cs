using Healthcare.Auth.Api.Core.Auth.Dtos.Request;
using FluentValidation;

namespace Healthcare.Auth.Api.Core.Auth.Validators
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("El refresh token es obligatorio.");
        }
    }
}
