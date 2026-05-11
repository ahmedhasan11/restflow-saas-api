using FluentValidation;
using RestflowAPI.DTOs.Customers;

namespace RestflowAPI.Validators.CustomerValidators
{
	public class CreateCustomerDtoValidator:AbstractValidator<CreateCustomerDto>
	{
		public CreateCustomerDtoValidator()
		{
			RuleFor(x => x.FullName)
				.NotEmpty().WithMessage("Full name is required.")
				.MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

			RuleFor(x => x.PhoneNumber)
				.NotEmpty().WithMessage("Phone number is required.")
				.Matches(@"^(\+20|0)?1[0125]\d{8}$").WithMessage("Invalid Egypt phone number format.");
		}
	}
}
