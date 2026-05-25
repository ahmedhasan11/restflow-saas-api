using RestflowAPI.DTOs.Employees;

namespace RestflowAPI.ServiceInterfaces.Employees
{
	public interface IEmployeesService
	{
		Task<List<EmployeeDto>> GetStaffListAsync(CancellationToken cancellationToken);
		Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto request, CancellationToken cancellationToken);

		Task<EmployeeDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);

		Task<EmployeeDto> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto request, CancellationToken cancellationToken);
	}
}
