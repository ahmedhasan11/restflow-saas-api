using RestflowAPI.DTOs.Employees;
using RestflowAPI.Entities;
using RestflowAPI.Enums;

namespace RestflowAPI.Repository.Interfaces.Employees
{
	public interface IEmployeesRepository
	{
		Task<List<EmployeeDto>> GetStaffListAsync(string? search, string? role, UserStatus? status, CancellationToken cancellationToken);
		Task<EmployeeDto?> GetByIdAsync(Guid employeeId, CancellationToken cancellationToken);

		Task AddAsync(Employee employee, CancellationToken cancellationToken);

		Task<Employee?> GetEntityByIdAsync(Guid employeeId, CancellationToken cancellationToken);
	}
}
