using FluentValidation;
using RestflowAPI.Constants;
using RestflowAPI.DTOs.Employees;

namespace RestflowAPI.Validators.EmployeesValidators
{
	public class UpdateEmployeeDtoValidator : AbstractValidator<UpdateEmployeeDto>
	{
		private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
		{
			Permissions.Roles.Owner,
			Permissions.Roles.Cashier,
			Permissions.Roles.Manager,
			Permissions.Roles.KitchenStaff,
			Permissions.Roles.InventoryClerk
		};
		public UpdateEmployeeDtoValidator()
		{
			RuleFor(x => x.FullName)
				.NotEmpty().WithMessage("Full name cannot be empty.")
				.MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.")
				.When(x => x.FullName != null);

			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email cannot be empty.")
				.EmailAddress().WithMessage("Invalid email format.")
				.When(x => x.Email != null);

			RuleFor(x => x.PhoneNumber)
				.NotEmpty().WithMessage("Phone number cannot be empty.")
				.Matches(@"^(\+20|0)?1[0125]\d{8}$").WithMessage("Invalid Egypt phone number format.")
				.When(x => x.PhoneNumber != null);

			RuleFor(x => x.Role)
				.NotEmpty().WithMessage("Role cannot be empty.")
				.Must(role => AllowedRoles.Contains(role!))
				.WithMessage($"Invalid role. Allowed roles: {string.Join(", ", AllowedRoles)}")
				.When(x => x.Role != null);

			RuleFor(x => x.Status)
				.IsInEnum().WithMessage("Invalid status value.")
				.When(x => x.Status != null);
		}
	}
}
