using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.Constants;
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
	}
}
