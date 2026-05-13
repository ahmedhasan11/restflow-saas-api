using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.DTOs.Settings;
using RestflowAPI.ServiceInterfaces.Settings;
using System.Security.Claims;

namespace RestflowAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class SettingsController : ControllerBase
	{
		private readonly ISettingsService _settingsService;

		public SettingsController(ISettingsService settingsService)
		{
			_settingsService = settingsService;
		}

		[HttpGet("profile")]
		public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
			{
				return Unauthorized();
			}

			var result = await _settingsService.GetUserProfileAsync(userId, cancellationToken);
			return Ok(result);
		}

		[HttpPatch("profile")]
		public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto request, CancellationToken cancellationToken)
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
			{
				return Unauthorized();
			}

			await _settingsService.UpdateProfileAsync(userId, request, cancellationToken);
			return Ok(new { message = "Profile updated successfully." });
		}
	}
}
