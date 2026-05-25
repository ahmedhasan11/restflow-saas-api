using RestflowAPI.DTOs.Employees;

namespace RestflowAPI.Repository.Interfaces.Employees
{
	public interface IEmployeesRepository
	{
		Task<List<EmployeeDto>> GetStaffListAsync(CancellationToken cancellationToken);
	}
}
