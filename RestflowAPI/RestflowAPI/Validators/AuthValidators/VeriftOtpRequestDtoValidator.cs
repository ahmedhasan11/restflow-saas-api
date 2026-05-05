using FluentValidation;
using RestflowAPI.DTOs.Auth;

namespace RestflowAPI.Validators.AuthValidators
{
	public class VerifyOtpRequestDtoValidator : AbstractValidator<VerifyOtpRequestDto>
	{
		public VerifyOtpRequestDtoValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("Invalid email format.");

			RuleFor(x => x.Code)
				.NotEmpty().WithMessage("OTP Code is required.")
				.Length(6).WithMessage("OTP Code must be 6 digits.");

			RuleFor(x => x.Channel)
				.IsInEnum().WithMessage("Invalid channel type.");
		}
	}
}
