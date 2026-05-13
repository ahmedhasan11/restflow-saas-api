using FluentValidation;
using RestflowAPI.DTOs.Settings;

namespace RestflowAPI.Validators.SettingsValidators
{
	public class UpdateProfileDtoValidator:AbstractValidator<UpdateProfileDto>
	{
		public UpdateProfileDtoValidator()
		{
			// FullName is only validated if it's not null/empty
			RuleFor(x => x.FullName)
				.MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.")
				.When(x => !string.IsNullOrEmpty(x.FullName));

			// PreferredLanguage is only validated if it's not null/empty
			RuleFor(x => x.PreferredLanguage)
				.Must(l => string.IsNullOrEmpty(l) || l == "en" || l == "ar")
				.WithMessage("Supported languages are 'en' and 'ar'.");
		}
	}
}
