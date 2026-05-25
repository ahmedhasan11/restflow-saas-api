using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.Employees;
using RestflowAPI.Repository.Employees;
using RestflowAPI.Repository.Interfaces.Employees;
using RestflowAPI.ServiceInterfaces.Employees;

namespace RestflowAPI.Services.Employees
{
	public class EmployeesService : IEmployeesService
	{
		private readonly IEmployeesRepository _employeesRepository;
		private readonly IUnitOfWork _unitOfWork;
		public EmployeesService(IEmployeesRepository employeesRepository, IUnitOfWork unitOfWork)
		{
			_employeesRepository = employeesRepository;
			_unitOfWork = unitOfWork;
		}

		public async Task<List<EmployeeDto>> GetStaffListAsync(CancellationToken cancellationToken)
		{
			return await _employeesRepository.GetStaffListAsync(cancellationToken);
		}
	}
}
