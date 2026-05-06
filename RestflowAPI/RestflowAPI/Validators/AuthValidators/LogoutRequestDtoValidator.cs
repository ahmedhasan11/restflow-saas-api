using FluentValidation;
using RestflowAPI.DTOs.Auth;

namespace RestflowAPI.Validators.AuthValidators
{
	public class LogoutRequestDtoValidator:AbstractValidator<LogoutRequestDto>
	{
		public LogoutRequestDtoValidator()
		{
			RuleFor(x => x.RefreshToken)
				.NotEmpty().WithMessage("Refresh token is required.");
		}
	}
}
