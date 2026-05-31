using FluentValidation;
using RestflowAPI.Constants;
using RestflowAPI.DTOs.Employees;

namespace RestflowAPI.Validators.EmployeesValidators
{
	public class CreateEmployeeDtoValidator:AbstractValidator<CreateEmployeeDto>
	{
		private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
		{
			Permissions.Roles.Owner,
			Permissions.Roles.Cashier,
			Permissions.Roles.Manager,
			Permissions.Roles.KitchenStaff,
			Permissions.Roles.InventoryClerk
		};
		public CreateEmployeeDtoValidator()
		{
			RuleFor(x => x.FullName)
				.NotEmpty().WithMessage("Full name is required.")
				.MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("Invalid email format.");

			RuleFor(x => x.PhoneNumber)
				.NotEmpty().WithMessage("Phone number is required.")
				.Matches(@"^(\+20|0)?1[0125]\d{8}$").WithMessage("Invalid Egypt phone number format.");

			RuleFor(x => x.Role)
				.NotEmpty().WithMessage("Role is required.")
				.Must(role => AllowedRoles.Contains(role))
				.WithMessage($"Invalid role. Allowed roles: {string.Join(", ", AllowedRoles)}");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required.")
				.MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
		}
	}

}
