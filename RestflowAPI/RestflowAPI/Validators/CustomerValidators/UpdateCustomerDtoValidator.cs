using FluentValidation;
using RestflowAPI.DTOs.Customers;

namespace RestflowAPI.Validators.CustomerValidators
{
	public class UpdateCustomerDtoValidator:AbstractValidator<UpdateCustomerDto>
	{
		public UpdateCustomerDtoValidator()
		{
			RuleFor(x => x.FullName)
				.NotEmpty().WithMessage("Full name cannot be empty.")
				.MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.")
				.When(x => x.FullName != null);

			RuleFor(x => x.PhoneNumber)
				.NotEmpty().WithMessage("Phone number cannot be empty.")
				.Matches(@"^(\+20|0)?1[0125]\d{8}$").WithMessage("Invalid Egypt phone number format.")
				.When(x => x.PhoneNumber != null);
		}
	}
}
