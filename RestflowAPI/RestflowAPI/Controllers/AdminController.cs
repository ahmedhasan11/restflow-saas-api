using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.Constants;
using RestflowAPI.DTOs.Auth;
using RestflowAPI.DTOs.Tenants;
using RestflowAPI.ServiceInterfaces.Auth;
using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Controllers
{
	[Authorize(Policy = Permissions.Policies.SuperAdminOnly)]
	[Route("api/[controller]")]
	[ApiController]
	public class AdminController : ControllerBase
	{
		private readonly ITenantService _tenantService;
		private readonly IAuthService _authService;

		public AdminController(ITenantService tenantService, IAuthService authService)
		{
			_tenantService = tenantService;
			_authService = authService;
		}

		[HttpPost("tenants")]
		public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _tenantService.CreateTenantAsync(request, cancellationToken);
			//lsa m5lsna4 hna 
			return Ok(result);
		}

		[HttpGet("tenants")]
		public async Task<IActionResult> GetAllTenants(CancellationToken cancellationToken)
		{
			var result = await _tenantService.GetAllTenantsAsync(cancellationToken);
			//lsa m5lsna4 hna
			return Ok(result);
		}

		[HttpPost("users")]
		public async Task<IActionResult> CreateUser([FromBody] CreateUserByAdminDto request, CancellationToken cancellationToken)
		{
			var result = await _authService.CreateUserByAdminAsync(request, cancellationToken);
			return Ok(result);
		}

		[HttpPatch("tenants/{id}/status")]
		public async Task<IActionResult> ChangeTenantStatus(Guid id, [FromBody] ChangeTenantStatusDto request, CancellationToken cancellationToken)
		{
			var result = await _tenantService.ChangeTenantStatusAsync(id, request, cancellationToken);
			return Ok(result);
		}
	}
}
