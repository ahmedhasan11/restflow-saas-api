using FluentValidation;
using RestflowAPI.DTOs.Employees;

namespace RestflowAPI.Validators.EmployeesValidators
{
	public class UpdateEmployeeStatusDtoValidator : AbstractValidator<UpdateEmployeeStatusDto>
	{
		public UpdateEmployeeStatusDtoValidator()
		{
			RuleFor(x => x.Status)
				.IsInEnum().WithMessage("Invalid status value.");
		}
	}
}
