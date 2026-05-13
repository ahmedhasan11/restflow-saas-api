using RestflowAPI.DTOs.Settings;

namespace RestflowAPI.ServiceInterfaces.Settings
{
	public interface ISettingsService
	{
		Task<UserProfileDto> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken);
		Task UpdateProfileAsync(Guid userId, UpdateProfileDto request, CancellationToken cancellationToken);

		Task<string> UploadProfileImageAsync(Guid userId, IFormFile file, CancellationToken cancellationToken);

		Task<NotificationSettingsDto> GetNotificationSettingsAsync(Guid userId, CancellationToken cancellationToken);

		Task UpdateNotificationSettingsAsync(Guid userId, NotificationSettingsDto request, CancellationToken cancellationToken);

		Task<RestaurantSettingsDto> GetRestaurantSettingsAsync(Guid userId, CancellationToken cancellationToken);

		Task UpdateRestaurantSettingsAsync(Guid userId, UpdateRestaurantSettingsDto request, CancellationToken cancellationToken);

		Task<string> UploadRestaurantLogoAsync(Guid userId, IFormFile file, CancellationToken cancellationToken);

		Task<PlatformSettingsDto> GetPlatformSettingsAsync(Guid userId, CancellationToken cancellationToken);

	}
}
