using RestflowAPI.DTOs.Settings;

namespace RestflowAPI.ServiceInterfaces.Settings
{
	public interface ISettingsService
	{
		Task<UserProfileDto> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken);
		Task UpdateProfileAsync(Guid userId, UpdateProfileDto request, CancellationToken cancellationToken);
	}
}
