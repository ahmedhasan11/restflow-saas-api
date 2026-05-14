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

			RuleFor(x => x.Country)
				.NotEmpty().WithMessage("Country is required.")
				.MaximumLength(100).WithMessage("Country cannot exceed 100 characters.");

			RuleFor(x => x.DefaultLanguage)
				.NotEmpty().WithMessage("Default language is required.")
				.Must(lang => lang == "en" || lang == "ar")
				.WithMessage("Supported languages are 'en' or 'ar'.");

			RuleFor(x => x.Timezone)
				.NotEmpty().WithMessage("Timezone is required.")
				.MaximumLength(50).WithMessage("Timezone cannot exceed 50 characters.");

			RuleFor(x => x.Currency)
				.NotEmpty().WithMessage("Currency is required.")
				.Length(3).WithMessage("Currency must be a 3-character code (e.g., USD, EGP).");
		}
	}
}
