using RestflowAPI.DTOs.Employees;

namespace RestflowAPI.ServiceInterfaces.Employees
{
	public interface IEmployeesService
	{
		Task<List<EmployeeDto>> GetStaffListAsync(CancellationToken cancellationToken);
		Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto request, CancellationToken cancellationToken);
	}
}
