using FluentValidation;
using RestflowAPI.DTOs.Settings;

namespace RestflowAPI.Validators.SettingsValidators
{
	public class UpdatePlatformSettingsDtoValidator : AbstractValidator<UpdatePlatformSettingsDto>
	{
		public UpdatePlatformSettingsDtoValidator()
		{
			RuleFor(x => x.SystemName)
				.MaximumLength(100).WithMessage("System name cannot exceed 100 characters.")
				.When(x => x.SystemName != null);

			RuleFor(x => x.SupportEmail)
				.EmailAddress().WithMessage("Invalid email format.")
				.When(x => !string.IsNullOrEmpty(x.SupportEmail));

			RuleFor(x => x.DefaultLanguage)
				.Must(lang => lang == "en" || lang == "ar")
				.WithMessage("Supported languages are 'en' or 'ar'.")
				.When(x => x.DefaultLanguage != null);

			RuleFor(x => x.CompanyName)
				.MaximumLength(150).WithMessage("Company name cannot exceed 150 characters.")
				.When(x => x.CompanyName != null);
		}
	}
}
