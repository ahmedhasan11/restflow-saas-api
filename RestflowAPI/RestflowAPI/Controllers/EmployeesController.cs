using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.Constants;
using RestflowAPI.DTOs.Employees;
using RestflowAPI.Enums;
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
		public async Task<ActionResult<List<EmployeeDto>>> GetStaffList(
			[FromQuery] string? search,
			[FromQuery] string? role,
			[FromQuery] UserStatus? status,
			CancellationToken cancellationToken)
		{
			var staff = await _employeesService.GetStaffListAsync(search, role, status, cancellationToken);
			return Ok(staff);
		}
		[HttpPost]
		public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeDto request, CancellationToken cancellationToken)
		{
			var employee = await _employeesService.CreateEmployeeAsync(request, cancellationToken);
			return CreatedAtAction(nameof(GetEmployeeDetails), new { id = employee.Id }, employee);
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<EmployeeDto>> GetEmployeeDetails([FromRoute] Guid id, CancellationToken cancellationToken)
		{
			var employee = await _employeesService.GetByIdAsync(id, cancellationToken);
			return Ok(employee);
		}
		[HttpPatch("{id}")]
		public async Task<ActionResult<EmployeeDto>> UpdateEmployee([FromRoute] Guid id, [FromBody] UpdateEmployeeDto request, CancellationToken cancellationToken)
		{
			var employee = await _employeesService.UpdateEmployeeAsync(id, request, cancellationToken);
			return Ok(employee);
		}
		[HttpPatch("{id}/status")]
		public async Task<ActionResult<EmployeeDto>> UpdateEmployeeStatus([FromRoute] Guid id, [FromBody] UpdateEmployeeStatusDto request, CancellationToken cancellationToken)
		{
			var employee = await _employeesService.UpdateStatusAsync(id, request, cancellationToken);
			return Ok(employee);
		}
	}
}
