using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.Constants;
using RestflowAPI.DTOs.Auth;
using RestflowAPI.DTOs.Common;
using RestflowAPI.DTOs.Settings;
using RestflowAPI.DTOs.Tenants;
using RestflowAPI.ServiceInterfaces.Auth;
using RestflowAPI.ServiceInterfaces.Settings;
using RestflowAPI.ServiceInterfaces.Tenants;
using System.Security.Claims;

namespace RestflowAPI.Controllers
{
	[Authorize(Policy = Permissions.Policies.SuperAdminOnly)]
	[Route("api/[controller]")]
	[ApiController]
	public class AdminController : ControllerBase
	{
		private readonly ITenantService _tenantService;
		private readonly IAuthService _authService;
		private readonly ISettingsService _settingsService;

		public AdminController(ITenantService tenantService, IAuthService authService, ISettingsService settingsService)
		{
			_tenantService = tenantService;
			_authService = authService;
			_settingsService = settingsService;
		}

		[HttpPost("tenants")]
		public async Task<ActionResult<TenantResponseDto>> CreateTenant([FromBody] CreateTenantRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _tenantService.CreateTenantAsync(request, cancellationToken);
			//lsa m5lsna4 hna 
			return Ok(result);
		}

		[HttpGet("tenants")]
		public async Task<ActionResult<IEnumerable<TenantResponseDto>>> GetAllTenants(CancellationToken cancellationToken)
		{
			var result = await _tenantService.GetAllTenantsAsync(cancellationToken);
			//lsa m5lsna4 hna
			return Ok(result);
		}


		[HttpPatch("tenants/{id}")]
		public async Task<ActionResult<TenantResponseDto>> UpdateTenant(Guid id, [FromBody] UpdateTenantRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _tenantService.UpdateTenantAsync(id, request, cancellationToken);
			return Ok(result);
		}

		[HttpPost("users")]
		public async Task<ActionResult<AuthResponseDto>> CreateUser([FromBody] CreateUserByAdminDto request, CancellationToken cancellationToken)
		{
			var result = await _authService.CreateUserByAdminAsync(request, cancellationToken);
			return Ok(result);
		}

		[HttpPatch("tenants/{id}/status")]
		public async Task<ActionResult<TenantResponseDto>> ChangeTenantStatus(Guid id, [FromBody] ChangeTenantStatusDto request, CancellationToken cancellationToken)
		{
			var result = await _tenantService.ChangeTenantStatusAsync(id, request, cancellationToken);
			return Ok(result);
		}

		[HttpGet("platform")]

		public async Task<ActionResult<PlatformSettingsDto>> GetPlatformSettings(CancellationToken cancellationToken)
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
			{
				return Unauthorized();
			}

			var result = await _settingsService.GetPlatformSettingsAsync(userId, cancellationToken);
			return Ok(result);
		}

		[HttpPatch("platform")]
		public async Task<ActionResult<MessageResponse>> UpdatePlatformSettings([FromBody] UpdatePlatformSettingsDto request, CancellationToken cancellationToken)
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
			{
				return Unauthorized();
			}

			await _settingsService.UpdatePlatformSettingsAsync(userId, request, cancellationToken);
			return Ok(new MessageResponse { Message = "Platform settings updated successfully." });
		}

		[HttpPatch("platform/api")]
		public async Task<ActionResult<MessageResponse>> UpdateApiSettings([FromBody] UpdatePlatformApiSettingsDto request, CancellationToken cancellationToken)
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
			{
				return Unauthorized();
			}

			await _settingsService.UpdatePlatformApiSettingsAsync(userId, request, cancellationToken);
			return Ok(new MessageResponse { Message = "API configurations updated successfully." });
		}
	}
}
