using RestflowAPI.DTOs.Employees;
using RestflowAPI.Enums;

namespace RestflowAPI.ServiceInterfaces.Employees
{
	public interface IEmployeesService
	{
		Task<List<EmployeeDto>> GetStaffListAsync(string? search, string? role, UserStatus? status, CancellationToken cancellationToken);
		Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto request, CancellationToken cancellationToken);

		Task<EmployeeDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);

		Task<EmployeeDto> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto request, CancellationToken cancellationToken);

		Task<EmployeeDto> UpdateStatusAsync(Guid id, UpdateEmployeeStatusDto request, CancellationToken cancellationToken);
	}
}
