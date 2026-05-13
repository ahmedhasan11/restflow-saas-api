using FluentValidation;
using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.Settings;
using RestflowAPI.Enums;
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
		private readonly IValidator<UpdateProfileDto> _updateProfileValidator;
		private readonly IUnitOfWork _unitOfWork;

		public SettingsService(ISettingsRepository settingsRepository, IAuthRepository authRepository, IValidator<UpdateProfileDto> updateProfileValidator, IUnitOfWork unitOfWork)
		{
			_settingsRepository = settingsRepository;
			_authRepository = authRepository;
			_updateProfileValidator = updateProfileValidator;
			_unitOfWork = unitOfWork;
		}
		public async Task<UserProfileDto> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken)
		{
			var user = await _authRepository.FindByIdAsync(userId, cancellationToken);
			if (user == null) throw new NotFoundException("User not found");

			if (user.Status != UserStatus.Active)
			{
				throw new UnauthorizedException(" account is inactive.");
			}
			return new UserProfileDto
			{
				FullName = user.FullName,
				ProfileImageUrl = user.ProfileImageUrl,
				PreferredLanguage = user.PreferredLanguage,
				Email = user.Email ?? string.Empty,
				PhoneNumber = user.PhoneNumber ?? string.Empty
			};
		}

		public async Task UpdateProfileAsync(Guid userId, UpdateProfileDto request, CancellationToken cancellationToken)
		{
			var result = await _updateProfileValidator.ValidateAsync(request, cancellationToken);
			if (!result.IsValid)
			{
				throw new AppValidationException(result.Errors.Select(e=>e.ErrorMessage));
			}

			var user = await _authRepository.FindByIdAsync(userId, cancellationToken);
			if (user == null)
			{
				throw new NotFoundException("User not found.");
			}
			if (user.Status != UserStatus.Active)
			{
				throw new UnauthorizedException("account is inactive.");
			}

			if (!string.IsNullOrWhiteSpace(request.FullName))
			{
				user.FullName = request.FullName;
			}
			if (!string.IsNullOrWhiteSpace(request.PreferredLanguage))
			{
				user.PreferredLanguage = request.PreferredLanguage;
			}
			user.UpdatedAt = DateTime.UtcNow;
			user.UpdatedBy = userId;

			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
	}
}
