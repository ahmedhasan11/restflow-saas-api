using RestflowAPI.DTOs.Settings;

namespace RestflowAPI.ServiceInterfaces.Settings
{
	public interface ISettingsService
	{
		Task<UserProfileDto> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken);
	}
}
