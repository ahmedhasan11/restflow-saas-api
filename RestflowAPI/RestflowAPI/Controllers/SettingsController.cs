using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
	}
}
