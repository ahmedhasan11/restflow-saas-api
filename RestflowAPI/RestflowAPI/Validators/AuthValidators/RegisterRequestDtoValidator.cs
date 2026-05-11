using FluentValidation;
using RestflowAPI.DTOs.Auth;

namespace RestflowAPI.Validators.AuthValidators
{
	public class RegisterRequestDtoValidator:AbstractValidator<RegisterRequestDto>
	{
		public RegisterRequestDtoValidator()
		{
			RuleFor(x => x.FullName)
				.NotEmpty().WithMessage("Full Name is required.")
				.MaximumLength(100).WithMessage("Full Name must not exceed 100 characters.");

			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("Invalid email format.");

			RuleFor(x => x.PhoneNumber)
				.NotEmpty().WithMessage("Phone Number is required.")
				.Matches(@"^(\+20|0)?1[0125]\d{8}$").WithMessage("Invalid Egypt phone number format.");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required.")
				.MinimumLength(5).WithMessage("Password must be at least 5 characters.")
				.Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.");

			RuleFor(x => x.ConfirmPassword)
				.NotEmpty().WithMessage("Confirm Password is required.")
				.Equal(x => x.Password).WithMessage("Passwords do not match.");
		}
	}
}
