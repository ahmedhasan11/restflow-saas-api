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
		private readonly IFileService _fileService;
		private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png" };
		private const long MaxImageSize = 2 * 1024 * 1024; // 2MB

		public SettingsService(ISettingsRepository settingsRepository, IAuthRepository authRepository, IValidator<UpdateProfileDto> updateProfileValidator, IUnitOfWork unitOfWork, IFileService fileService)
		{
			_settingsRepository = settingsRepository;
			_authRepository = authRepository;
			_updateProfileValidator = updateProfileValidator;
			_unitOfWork = unitOfWork;
			_fileService = fileService;
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

		public async Task<string> UploadProfileImageAsync(Guid userId, IFormFile file, CancellationToken cancellationToken)
		{
			// 1. Validation
			if (file == null || file.Length == 0)
				throw new AppValidationException("Please select an image to upload.");

			var extension = Path.GetExtension(file.FileName).ToLower();
			if (!AllowedImageExtensions.Contains(extension))
				throw new AppValidationException("Invalid file format. Supported formats are: .jpg, .jpeg, .png");
			if (file.Length > MaxImageSize)
				throw new AppValidationException("Image size exceeds the 2MB limit.");

			var user = await _authRepository.FindByIdAsync(userId, cancellationToken);
			if (user == null)
			{
				throw new NotFoundException("User not found.");
			}
			if (user.Status != UserStatus.Active)
			{
				throw new UnauthorizedException("Account is inactive.");
			}
			// 3. Delete old image if exists to save space
			if (!string.IsNullOrEmpty(user.ProfileImageUrl))
			{
				_fileService.DeleteFile(user.ProfileImageUrl);
			}
			var imageUrl = await _fileService.UploadFileAsync(file, "profile_images", cancellationToken);

			user.ProfileImageUrl = imageUrl;
			user.UpdatedAt = DateTime.UtcNow;
			user.UpdatedBy = userId;

			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return imageUrl;
		}

		public async Task<NotificationSettingsDto> GetNotificationSettingsAsync(Guid userId, CancellationToken cancellationToken)
		{
			var user = await _authRepository.FindByIdAsync(userId, cancellationToken);
			if (user == null) throw new NotFoundException("User not found.");
			if (user.Status != UserStatus.Active)
			{
				throw new UnauthorizedException("Account is inactive.");
			}

			if (string.IsNullOrEmpty(user.NotificationPreferences))
			{
				// Return default settings if none are stored yet
				return new NotificationSettingsDto();
			}

			try
			{
				return System.Text.Json.JsonSerializer.Deserialize<NotificationSettingsDto>(user.NotificationPreferences)
					   ?? new NotificationSettingsDto();
			}
			catch
			{
				// Fallback to defaults if JSON is corrupted
				return new NotificationSettingsDto();
			}
		}
	}
}
