using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.Constants;
using RestflowAPI.DTOs.Customers;
using RestflowAPI.ServiceInterfaces.Customers;

namespace RestflowAPI.Controllers
{
	[Authorize(Policy = Permissions.Policies.TenantAccess)]
	[Route("api/[controller]")]
	[ApiController]
	public class CustomersController : ControllerBase
	{
		private readonly ICustomerService _customerService;
		public CustomersController(ICustomerService customerService)
		{
			_customerService = customerService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
		{
			var customers = await _customerService.GetAllAsync(cancellationToken);
			return Ok(customers);
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateCustomerDto dto, CancellationToken cancellationToken)
		{
			var result = await _customerService.CreateAsync(dto, cancellationToken);
			return Ok(result);
		}
	}
}
