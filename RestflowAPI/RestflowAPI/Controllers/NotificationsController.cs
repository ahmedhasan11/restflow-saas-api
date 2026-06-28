using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.Constants;
using RestflowAPI.DTOs.Notifications;
using RestflowAPI.ServiceInterfaces.Notifications;
using System.Security.Claims;

namespace RestflowAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = Permissions.Policies.TenantAccess)]
	public class NotificationsController : ControllerBase
	{
		private readonly INotificationsService _notificationsService;
		public NotificationsController(INotificationsService notificationsService)
		{
			_notificationsService = notificationsService;
		}

		[HttpGet]
		public async Task<ActionResult<NotificationListResponseDto>> GetNotifications(
	[FromQuery] GetNotificationsRequestDto query,	CancellationToken ct = default)
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
			{
				return Unauthorized();
			}

			var result = await _notificationsService.GetUserNotificationsAsync(userId, query, ct);
			return Ok(result);
		}
	}
}
