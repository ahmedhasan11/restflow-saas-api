using FluentValidation;
using RestflowAPI.DTOs.Auth;

namespace RestflowAPI.Validators.AuthValidators
{
	public class CreateUserByAdminDtoValidator:AbstractValidator<CreateUserByAdminDto>
	{
		public CreateUserByAdminDtoValidator()
		{
			RuleFor(x => x.FullName).NotEmpty();
			RuleFor(x => x.Email).NotEmpty().EmailAddress();
			RuleFor(x => x.PhoneNumber).NotEmpty();
			RuleFor(x => x.Password).NotEmpty().MinimumLength(5);
			RuleFor(x => x.Role).Must(r => r == Enums.UserRole.Owner || r == Enums.UserRole.Employee)
				.WithMessage("Only Owner or Employee roles can be created by admin.");
			RuleFor(x => x.TenantId).NotEmpty().WithMessage("TenantId is required.");
		}
	}
}
