using FluentValidation;
using RestflowAPI.DTOs.Settings;

namespace RestflowAPI.Validators.SettingsValidators
{
	public class UpdateRestaurantSettingsDtoValidator : AbstractValidator<UpdateRestaurantSettingsDto>
	{
		public UpdateRestaurantSettingsDtoValidator()
		{
			RuleFor(x => x.RestaurantName)
				.NotEmpty().WithMessage("Restaurant name cannot be empty.")
				.MaximumLength(100).WithMessage("Restaurant name cannot exceed 100 characters.")
				.When(x => x.RestaurantName != null);

			RuleFor(x => x.CuisineType)
				.MaximumLength(50).WithMessage("Cuisine type cannot exceed 50 characters.")
				.When(x => x.CuisineType != null);
		}
	}
}
