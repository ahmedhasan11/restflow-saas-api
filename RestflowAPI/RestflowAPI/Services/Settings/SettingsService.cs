using RestflowAPI.DTOs.Settings;
using RestflowAPI.Exceptions;
using RestflowAPI.Repository.Interfaces.Auth;
using RestflowAPI.Repository.Interfaces.Settings;
using RestflowAPI.ServiceInterfaces.Settings;

namespace RestflowAPI.Services.Settings
{
	public class SettingsService : ISettingsService
	{
		private readonly ISettingsRepository _settingsRepository;
		private readonly IAuthRepository _authRepository;

		public SettingsService(ISettingsRepository settingsRepository, IAuthRepository authRepository)
		{
			_settingsRepository = settingsRepository;
			_authRepository = authRepository;
		}
		public async Task<UserProfileDto> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken)
		{
			var user = await _authRepository.FindByIdAsync(userId, cancellationToken);
			if (user == null) throw new NotFoundException("User not found");


			return new UserProfileDto
			{
				FullName = user.FullName,
				ProfileImageUrl = user.ProfileImageUrl,
				PreferredLanguage = user.PreferredLanguage,
				Email = user.Email ?? string.Empty,
				PhoneNumber = user.PhoneNumber ?? string.Empty
			};
		}
	}
}
