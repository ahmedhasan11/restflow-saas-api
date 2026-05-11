using FluentValidation;
using RestflowAPI.DTOs.Auth;
using System.Text.RegularExpressions;

namespace RestflowAPI.Validators.AuthValidators
{
	public class ResetPasswordRequestDtoValidator:AbstractValidator<ResetPasswordRequestDto>
	{
		public ResetPasswordRequestDtoValidator()
		{
			RuleFor(x => x.Identifier)
				.NotEmpty().WithMessage("Email or Egyptian Phone Number is required.")
				.Must(BeAValidEmailOrEgyptianPhone).WithMessage("Please enter a valid email address or Egyptian phone number.");

			RuleFor(x => x.OtpCode)
				.NotEmpty().WithMessage("OTP code is required.")
				.Length(6).WithMessage("OTP code must be 6 digits.");

			RuleFor(x => x.NewPassword)
				.NotEmpty().WithMessage("New password is required.")
				.MinimumLength(5).WithMessage("Password must be at least 5 characters.");
		}

		private bool BeAValidEmailOrEgyptianPhone(string identifier)
		{
			if (string.IsNullOrWhiteSpace(identifier)) return false;
			var emailRegex = new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$", RegexOptions.IgnoreCase);
			var egyptPhoneRegex = new Regex(@"^(\+20|20)?(010|011|012|015)\d{8}$");
			return emailRegex.IsMatch(identifier) || egyptPhoneRegex.IsMatch(identifier);
		}
	}
}
