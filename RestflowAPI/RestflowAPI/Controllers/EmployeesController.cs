using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.Constants;
using RestflowAPI.DTOs.Employees;
using RestflowAPI.ServiceInterfaces.Employees;
using RestflowAPI.Services.Employees;

namespace RestflowAPI.Controllers
{
	[Authorize(Policy = Permissions.Policies.OwnerOnly)]
	[Route("api/[controller]")]
	[ApiController]
	public class EmployeesController : ControllerBase
	{
		private readonly IEmployeesService _employeesService;

		public EmployeesController(IEmployeesService employeesService)
		{
			_employeesService = employeesService;
		}

		[HttpGet]
		public async Task<IActionResult> GetStaffList(CancellationToken cancellationToken)
		{
			var staff = await _employeesService.GetStaffListAsync(cancellationToken);
			return Ok(staff);
		}
		[HttpPost]
		public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto request, CancellationToken cancellationToken)
		{
			var employee = await _employeesService.CreateEmployeeAsync(request, cancellationToken);
			return CreatedAtAction(nameof(GetEmployeeDetails), new { id = employee.Id }, employee);
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetEmployeeDetails([FromRoute] Guid id, CancellationToken cancellationToken)
		{
			var employee = await _employeesService.GetByIdAsync(id, cancellationToken);
			return Ok(employee);
		}

		[HttpPatch("{id}")]
		public async Task<IActionResult> UpdateEmployee([FromRoute] Guid id, [FromBody] UpdateEmployeeDto request, CancellationToken cancellationToken)
		{
			var employee = await _employeesService.UpdateEmployeeAsync(id, request, cancellationToken);
			return Ok(employee);
		}
	}
}
