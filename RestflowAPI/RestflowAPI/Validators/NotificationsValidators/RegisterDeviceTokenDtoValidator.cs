using FluentValidation;
using RestflowAPI.DTOs.Notifications;

namespace RestflowAPI.Validators.NotificationsValidators
{
	public class RegisterDeviceTokenDtoValidator : AbstractValidator<RegisterDeviceTokenDto>
	{
		public RegisterDeviceTokenDtoValidator()
		{
			RuleFor(x => x.Token)
				.NotEmpty().WithMessage("Device token is required.")
				.MaximumLength(1000).WithMessage("Device token must not exceed 1000 characters.");

			RuleFor(x => x.DeviceType)
				.IsInEnum().WithMessage("Device type must be Web, Android, or iOS.");
		}
	}
}
