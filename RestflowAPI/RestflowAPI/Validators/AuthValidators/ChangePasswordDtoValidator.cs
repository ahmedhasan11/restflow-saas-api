using FluentValidation;
using RestflowAPI.DTOs.Auth;

namespace RestflowAPI.Validators.AuthValidators
{
	public class ChangePasswordDtoValidator:AbstractValidator<ChangePasswordDto>
	{
		public ChangePasswordDtoValidator()
		{
			RuleFor(x => x.CurrentPassword)
				.NotEmpty().WithMessage("Current password is required.");

			RuleFor(x => x.NewPassword)
				.NotEmpty().WithMessage("New password is required.")
				.MinimumLength(5).WithMessage("New password must be at least 5 characters.")
				.NotEqual(x => x.CurrentPassword).WithMessage("New password cannot be the same as the current password.");

			RuleFor(x => x.ConfirmNewPassword)
				.Equal(x => x.NewPassword)
				.WithMessage("New password and confirmation do not match.");
		}
	}
}
