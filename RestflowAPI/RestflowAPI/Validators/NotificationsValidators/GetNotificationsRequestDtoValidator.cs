using FluentValidation;
using RestflowAPI.DTOs.Notifications;

namespace RestflowAPI.Validators.NotificationsValidators
{
	public class GetNotificationsRequestDtoValidator : AbstractValidator<GetNotificationsRequestDto>
	{
		public GetNotificationsRequestDtoValidator()
		{
			RuleFor(x => x.Page)
				.GreaterThanOrEqualTo(1).WithMessage("Page number must be greater than or equal to 1.");

			RuleFor(x => x.PageSize)
				.InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
		}
	}
}
