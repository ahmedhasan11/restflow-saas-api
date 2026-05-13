using FluentValidation;
using RestflowAPI.DTOs.Settings;

namespace RestflowAPI.Validators.SettingsValidators
{
	public class UpdateRestaurantSettingsDtoValidator : AbstractValidator<UpdateRestaurantSettingsDto>
	{
		public UpdateRestaurantSettingsDtoValidator()
		{
			RuleFor(x => x.RestaurantName)
				.MaximumLength(100).WithMessage("Restaurant name cannot exceed 100 characters.")
				.When(x => x.RestaurantName != null);

			RuleFor(x => x.CuisineType)
				.MaximumLength(50).WithMessage("Cuisine type cannot exceed 50 characters.")
				.When(x => x.CuisineType != null);

			RuleFor(x => x.Country)
				.MaximumLength(100).WithMessage("Country cannot exceed 100 characters.")
				.When(x => x.Country != null);

			RuleFor(x => x.DefaultLanguage)
				.Must(lang => lang == "en" || lang == "ar")
				.WithMessage("Supported languages are 'en' or 'ar'.")
				.When(x => x.DefaultLanguage != null);

			RuleFor(x => x.Timezone)
				.MaximumLength(50).WithMessage("Timezone cannot exceed 50 characters.")
				.When(x => x.Timezone != null);

			RuleFor(x => x.Currency)
				.Length(3).WithMessage("Currency must be a 3-character code (e.g., USD, EGP).")
				.When(x => x.Currency != null);
		}
	}
}
