using FluentValidation;
using RestflowAPI.DTOs.Auth;
using System.Text.RegularExpressions;

namespace RestflowAPI.Validators.AuthValidators
{
	public class LoginRequestDtoValidator:AbstractValidator<LoginRequestDto>
	{
		public LoginRequestDtoValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email or Phone Number is required.")
				.Must(BeAValidEmailOrEgyptianPhone).WithMessage("Please enter a valid email address or phone number.");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required.");
		}
		private bool BeAValidEmailOrEgyptianPhone(string identifier)
		{
			if (string.IsNullOrWhiteSpace(identifier)) return false;
			// الـ Email Regex القوي (RFC 5322)
			var emailRegex = new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$", RegexOptions.IgnoreCase);

			// الـ Phone Regex المظبوط لمصر (010, 011, 012, 015)
			var egyptPhoneRegex = new Regex(@"^(\+20|20)?(010|011|012|015)\d{8}$");
			return emailRegex.IsMatch(identifier) || egyptPhoneRegex.IsMatch(identifier);
		}
	}
}
