using FluentValidation;
using RestflowAPI.DTOs.Customers;

namespace RestflowAPI.Validators.CustomerValidators
{
	public class UpdateCustomerStatusDtoValidator : AbstractValidator<UpdateCustomerStatusDto>
	{
		public UpdateCustomerStatusDtoValidator()
		{
			RuleFor(x => x.Status)
				.IsInEnum().WithMessage("Invalid status value.");
		}
	}
}
