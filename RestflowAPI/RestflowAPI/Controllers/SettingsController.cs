



using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.Constants;
using RestflowAPI.DTOs.Common;
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
		public async Task<ActionResult<UserProfileDto>> GetProfile(CancellationToken cancellationToken)
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
		public async Task<ActionResult<MessageResponse>> UpdateProfile([FromBody] UpdateProfileDto request, CancellationToken cancellationToken)
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
			{
				return Unauthorized();
			}

			await _settingsService.UpdateProfileAsync(userId, request, cancellationToken);
			return Ok(new MessageResponse{ Message = "Profile updated successfully." });
		}

		[HttpPost("profile/image")]
		[Consumes("multipart/form-data")]
		public async Task<ActionResult<ImageUploadResponse>> UploadProfileImage(IFormFile file, CancellationToken cancellationToken)
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
			{
				return Unauthorized();
			}

			var imageUrl = await _settingsService.UploadProfileImageAsync(userId, file, cancellationToken);
			return Ok(new ImageUploadResponse{ ImageUrl =  imageUrl, Message = "Profile image uploaded successfully." });
		}

		[HttpGet("notifications")]
		public async Task<ActionResult<NotificationSettingsDto>> GetNotificationSettings(CancellationToken cancellationToken)
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
			{
				return Unauthorized();
			}

			var result = await _settingsService.GetNotificationSettingsAsync(userId, cancellationToken);
			return Ok(result);
		}

		[HttpPatch("notifications")]
		public async Task<ActionResult<MessageResponse>> UpdateNotificationSettings([FromBody] NotificationSettingsDto request, CancellationToken cancellationToken)
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
			{
				return Unauthorized();
			}

			await _settingsService.UpdateNotificationSettingsAsync(userId, request, cancellationToken);
			return Ok(new MessageResponse{ Message = "Notification preferences updated successfully." });
		}

		[HttpGet("restaurant")]
		[Authorize(Policy = Permissions.Policies.OwnerOnly)]
		public async Task<ActionResult<RestaurantSettingsDto>> GetRestaurantSettings(CancellationToken cancellationToken)
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
			{
				return Unauthorized();
			}

			var result = await _settingsService.GetRestaurantSettingsAsync(userId, cancellationToken);
			return Ok(result);
		}

		[HttpPatch("restaurant")]
		[Authorize(Policy = Permissions.Policies.OwnerOnly)]
		public async Task<ActionResult<MessageResponse>> UpdateRestaurantSettings([FromBody] UpdateRestaurantSettingsDto request, CancellationToken cancellationToken)
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
			{
				return Unauthorized();
			}

			await _settingsService.UpdateRestaurantSettingsAsync(userId, request, cancellationToken);
			return Ok(new MessageResponse { Message = "Restaurant settings updated successfully." });
		}

		[HttpPost("restaurant/logo")]
		[Authorize(Policy = Permissions.Policies.OwnerOnly)]
		[Consumes("multipart/form-data")]
		public async Task<ActionResult<LogoUploadResponse>> UploadRestaurantLogo(IFormFile file, CancellationToken cancellationToken)
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
			{
				return Unauthorized();
			}

			var logoUrl = await _settingsService.UploadRestaurantLogoAsync(userId, file, cancellationToken);
			return Ok(new LogoUploadResponse {LogoUrl= logoUrl, Message = "Restaurant logo uploaded successfully." });
		}


	}
}
