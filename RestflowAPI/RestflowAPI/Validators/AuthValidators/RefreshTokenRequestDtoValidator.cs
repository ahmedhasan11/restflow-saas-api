using FluentValidation;
using RestflowAPI.DTOs.Auth;

namespace RestflowAPI.Validators.AuthValidators
{
	public class RefreshTokenRequestDtoValidator:AbstractValidator<RefreshTokenRequestDto>
	{
		public RefreshTokenRequestDtoValidator()
		{
			RuleFor(x => x.RefreshToken)
				.NotEmpty()
				.WithMessage("Refresh token is required.");
		}
	}
}
