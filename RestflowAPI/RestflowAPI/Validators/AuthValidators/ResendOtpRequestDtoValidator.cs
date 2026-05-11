using FluentValidation;
using RestflowAPI.DTOs.Auth;

namespace RestflowAPI.Validators.AuthValidators
{
	public class ResendOtpRequestDtoValidator : AbstractValidator<ResendOtpRequestDto>
	{
		public ResendOtpRequestDtoValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("Invalid email format.");

			RuleFor(x => x.Channel)
				.IsInEnum().WithMessage("Invalid channel type.");
		}
	}
}
