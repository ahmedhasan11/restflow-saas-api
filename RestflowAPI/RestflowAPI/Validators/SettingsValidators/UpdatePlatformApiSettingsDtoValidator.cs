using FluentValidation;
using RestflowAPI.DTOs.Settings;

namespace RestflowAPI.Validators.SettingsValidators
{
	public class UpdatePlatformApiSettingsDtoValidator: AbstractValidator<UpdatePlatformApiSettingsDto>
	{
		public UpdatePlatformApiSettingsDtoValidator()
		{
			RuleFor(x => x.Settings)
				.NotEmpty().WithMessage("At least one setting must be provided.");

			RuleForEach(x => x.Settings.Keys)
				.NotEmpty().WithMessage("Setting key cannot be empty.");
		}
	}
}
