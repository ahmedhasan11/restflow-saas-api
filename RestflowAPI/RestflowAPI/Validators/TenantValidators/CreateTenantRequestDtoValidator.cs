using FluentValidation;
using RestflowAPI.DTOs.Tenants;

namespace RestflowAPI.Validators.TenantValidators
{
	public class CreateTenantRequestDtoValidator:AbstractValidator<CreateTenantRequestDto>
	{
		public CreateTenantRequestDtoValidator()
		{
		
			RuleFor(x => x.RestaurantName)
				.NotEmpty().WithMessage("Restaurant name is required.")
				.MaximumLength(100).WithMessage("Restaurant name cannot exceed 100 characters.");

			RuleFor(x => x.TenantCode)
				.NotEmpty().WithMessage("Tenant code is required.")
				.MaximumLength(50).WithMessage("Tenant code cannot exceed 50 characters.")
				.Matches(@"^[a-z0-9-]+$").WithMessage("Tenant code can only contain lowercase letters, numbers, and hyphens.");
		
		}
	}
}
