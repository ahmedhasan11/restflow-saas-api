using FluentValidation;
using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.Settings;
using RestflowAPI.Entities;
using RestflowAPI.Enums;
using RestflowAPI.Exceptions;
using RestflowAPI.Repository.Interfaces.Auth;
using RestflowAPI.Repository.Interfaces.Settings;
using RestflowAPI.Repository.Interfaces.Tenants;
using RestflowAPI.Repository.Settings;
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
		private readonly ITenantRepository _tenantRepository;
		private readonly IValidator<UpdateRestaurantSettingsDto> _updateRestaurantSettingsDto;
		private readonly IValidator<UpdatePlatformSettingsDto> _updatePlatformSettingsDto;
		private readonly IValidator<UpdatePlatformApiSettingsDto> _updatePlatformApiSettingsDto;
		private readonly IPlatformRepository _platformRepository;
		private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png" };
		private const long MaxImageSize = 2 * 1024 * 1024; // 2MB

		public SettingsService(ISettingsRepository settingsRepository, IAuthRepository authRepository, IValidator<UpdateProfileDto> updateProfileValidator,
			IUnitOfWork unitOfWork, IFileService fileService, ITenantRepository tenantRepository, IValidator<UpdateRestaurantSettingsDto> updateRestaurantSettingsDto
			, IPlatformRepository platformRepository, IValidator<UpdatePlatformSettingsDto> updatePlatformSettingsDto, IValidator<UpdatePlatformApiSettingsDto> updatePlatformApiSettingsDto)
		{
			_settingsRepository = settingsRepository;
			_authRepository = authRepository;
			_updateProfileValidator = updateProfileValidator;
			_unitOfWork = unitOfWork;
			_fileService = fileService;
			_tenantRepository = tenantRepository;
			_updateRestaurantSettingsDto = updateRestaurantSettingsDto;
			_platformRepository = platformRepository;
			_updatePlatformSettingsDto = updatePlatformSettingsDto;
			_updatePlatformApiSettingsDto = updatePlatformApiSettingsDto;
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
			// 1. Store the old path
			var oldImagePath = user.ProfileImageUrl;
			var imageUrl = await _fileService.UploadFileAsync(file, "profile_images", cancellationToken);

			user.ProfileImageUrl = imageUrl;
			user.UpdatedAt = DateTime.UtcNow;
			user.UpdatedBy = userId;

			await _unitOfWork.SaveChangesAsync(cancellationToken);

			// 4. Only delete the old image IF the upload and save were successful
			if (!string.IsNullOrEmpty(oldImagePath))
			{
				_fileService.DeleteFile(oldImagePath);
			}

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

			// Define System Defaults here
			var defaultSettings = new NotificationSettingsDto
			{
				EmailNotifications = true,
				InAppNotifications = true,
				AiInsightsNotifications = false,
				InventoryAlerts = true,
				ImportantAlerts = true
			};
			if (string.IsNullOrEmpty(user.NotificationPreferences))
			{
				// Return default settings if none are stored yet
				return defaultSettings;
			}

			try
			{
				var savedSettings = System.Text.Json.JsonSerializer.Deserialize<NotificationSettingsDto>(user.NotificationPreferences);

				// If the saved settings are valid, merge them with defaults (just in case some fields are missing)
				if (savedSettings != null)
				{
					return new NotificationSettingsDto
					{
						EmailNotifications = savedSettings.EmailNotifications ?? defaultSettings.EmailNotifications,
						InAppNotifications = savedSettings.InAppNotifications ?? defaultSettings.InAppNotifications,
						AiInsightsNotifications = savedSettings.AiInsightsNotifications ?? defaultSettings.AiInsightsNotifications,
						InventoryAlerts = savedSettings.InventoryAlerts ?? defaultSettings.InventoryAlerts,
						ImportantAlerts = savedSettings.ImportantAlerts ?? defaultSettings.ImportantAlerts
					};
				}
				return defaultSettings;
			}
			catch
			{
				// Fallback to defaults if JSON is corrupted
				return defaultSettings;
			}
		}

		public async Task UpdateNotificationSettingsAsync(Guid userId, NotificationSettingsDto request, CancellationToken cancellationToken)
		{
			var user = await _authRepository.FindByIdAsync(userId, cancellationToken);
			if (user == null )
			{
				throw new NotFoundException("User not found ");
			}
			if(user.Status != UserStatus.Active)
			{
				throw new UnauthorizedException("account is inactive.");
			}

			// 1. Get current settings (or defaults if none exist)
			var currentSettings = await GetNotificationSettingsAsync(userId, cancellationToken);

			// 2. Merge changes: Only update the fields that the user actually sent (not null)
			if (request.EmailNotifications.HasValue)
				currentSettings.EmailNotifications = request.EmailNotifications.Value;

			if (request.InAppNotifications.HasValue)
				currentSettings.InAppNotifications = request.InAppNotifications.Value;

			if (request.AiInsightsNotifications.HasValue)
				currentSettings.AiInsightsNotifications = request.AiInsightsNotifications.Value;

			if (request.InventoryAlerts.HasValue)
				currentSettings.InventoryAlerts = request.InventoryAlerts.Value;

			if (request.ImportantAlerts.HasValue)
				currentSettings.ImportantAlerts = request.ImportantAlerts.Value;

			// 3. Convert the merged object back into a JSON string
			user.NotificationPreferences = System.Text.Json.JsonSerializer.Serialize(currentSettings);
			user.UpdatedAt = DateTime.UtcNow;
			user.UpdatedBy = userId;

			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}

		public async Task<RestaurantSettingsDto> GetRestaurantSettingsAsync(Guid userId, CancellationToken cancellationToken)
		{
			var user = await _authRepository.FindByIdAsync(userId, cancellationToken);
			if (user == null)
			{
				throw new NotFoundException("User not found ");
			}
			if (user.Status != UserStatus.Active)
			{
				throw new UnauthorizedException("account is inactive.");
			}

			if (!user.TenantId.HasValue)
			{
				throw new ForbiddenException("User is not associated with a restaurant.");
			}

			var tenant = await _tenantRepository.GetByIdAsync(user.TenantId.Value, cancellationToken);
			if (tenant == null)
			{
				throw new NotFoundException("Restaurant settings not found.");
			}

			return new RestaurantSettingsDto
			{
				RestaurantName = tenant.RestaurantName,
				RestaurantLogoUrl = tenant.RestaurantLogoUrl,
				CuisineType = tenant.CuisineType
			};
		}

		public async Task UpdateRestaurantSettingsAsync(Guid userId, UpdateRestaurantSettingsDto request, CancellationToken cancellationToken)
		{
			var validationResult = await _updateRestaurantSettingsDto.ValidateAsync(request, cancellationToken);
			if (!validationResult.IsValid)
			{
				throw new AppValidationException(validationResult.Errors.Select(e => e.ErrorMessage));
			}

			var user = await _authRepository.FindByIdAsync(userId, cancellationToken);
			if (user == null)
			{
				throw new NotFoundException("User not found ");
			}
			if (user.Status != UserStatus.Active)
			{
				throw new UnauthorizedException("account is inactive.");
			}

			if (!user.TenantId.HasValue)
			{
				throw new ForbiddenException("User is not associated with a restaurant.");
			}

			var tenant = await _tenantRepository.GetByIdAsync(user.TenantId.Value, cancellationToken);
			if (tenant == null)
			{
				throw new NotFoundException("Restaurant not found.");
			}

			if (!string.IsNullOrWhiteSpace(request.RestaurantName))
			{
				tenant.RestaurantName = request.RestaurantName;
			}

			if (!string.IsNullOrWhiteSpace(request.CuisineType))
			{
				tenant.CuisineType = request.CuisineType;
			}

			tenant.UpdatedAt = DateTime.UtcNow;
			tenant.UpdatedBy = userId;

			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}

		public async Task<string> UploadRestaurantLogoAsync(Guid userId, IFormFile file, CancellationToken cancellationToken)
		{
			if (file == null || file.Length == 0)
				throw new AppValidationException("Please select a logo to upload.");

			var extension = Path.GetExtension(file.FileName).ToLower();
			if (!AllowedImageExtensions.Contains(extension))
				throw new AppValidationException("Invalid file format. Supported formats are: .jpg, .jpeg, .png");

			if (file.Length > MaxImageSize)
				throw new AppValidationException("Logo size exceeds the 2MB limit.");

			var user = await _authRepository.FindByIdAsync(userId, cancellationToken);
			if (user == null)
			{
				throw new NotFoundException("User not found ");
			}
			if (user.Status != UserStatus.Active)
			{
				throw new UnauthorizedException("account is inactive.");
			}

			if (!user.TenantId.HasValue)
				throw new ForbiddenException("User is not associated with a restaurant.");


			var tenant = await _tenantRepository.GetByIdAsync(user.TenantId.Value, cancellationToken);
			if (tenant == null)
				throw new NotFoundException("Restaurant not found.");

			var oldLogoPath = tenant.RestaurantLogoUrl;

			var logoUrl = await _fileService.UploadFileAsync(file, "logos", cancellationToken);


			tenant.RestaurantLogoUrl = logoUrl;
			tenant.UpdatedAt = DateTime.UtcNow;
			tenant.UpdatedBy = userId;

			await _unitOfWork.SaveChangesAsync(cancellationToken);

			// 4. Only delete the old logo IF the upload and save were successful
			if (!string.IsNullOrEmpty(oldLogoPath))
			{
				_fileService.DeleteFile(oldLogoPath);
			}
			return logoUrl;
		}

		public async Task<PlatformSettingsDto> GetPlatformSettingsAsync(Guid userId, CancellationToken cancellationToken)
		{
			var user = await _authRepository.FindByIdAsync(userId, cancellationToken);
			if (user == null)
			{
				throw new NotFoundException("User not found ");
			}
			if (user.Status != UserStatus.Active)
			{
				throw new UnauthorizedException("account is inactive.");
			}

			var settings = await _platformRepository.GetAllAsync(cancellationToken);
			var dto = new PlatformSettingsDto();
			foreach (var setting in settings)
			{
				switch (setting.SettingKey)
				{
					case "SystemName": dto.SystemName = setting.SettingValue; break;
					case "SystemLogoUrl": dto.SystemLogoUrl = setting.SettingValue; break;
					case "DefaultLanguage": dto.DefaultLanguage = setting.SettingValue; break;
					case "SupportEmail": dto.SupportEmail = setting.SettingValue; break;
					case "CompanyName": dto.CompanyName = setting.SettingValue; break;
					default:
						dto.ApiConfigurations[setting.SettingKey] = setting.SettingValue;
						break;
				}
			}

			return dto;
		}

		public async Task UpdatePlatformSettingsAsync(Guid userId, UpdatePlatformSettingsDto request, CancellationToken cancellationToken)
		{
			var validationResult = await _updatePlatformSettingsDto.ValidateAsync(request, cancellationToken);
			if (!validationResult.IsValid)
			{
				throw new AppValidationException(validationResult.Errors.Select(e => e.ErrorMessage));
			}

			var user = await _authRepository.FindByIdAsync(userId, cancellationToken);
			if (user == null)
			{
				throw new NotFoundException("User not found ");
			}
			if (user.Status != UserStatus.Active)
			{
				throw new UnauthorizedException("account is inactive.");
			}

			if (!string.IsNullOrWhiteSpace(request.SystemName))
				await SavePlatformSetting("SystemName", request.SystemName, false, userId, cancellationToken);

			if (!string.IsNullOrWhiteSpace(request.SystemLogoUrl))
				await SavePlatformSetting("SystemLogoUrl", request.SystemLogoUrl, false, userId, cancellationToken);

			if (!string.IsNullOrWhiteSpace(request.DefaultLanguage))
				await SavePlatformSetting("DefaultLanguage", request.DefaultLanguage, false, userId, cancellationToken);

			if (!string.IsNullOrWhiteSpace(request.SupportEmail))
				await SavePlatformSetting("SupportEmail", request.SupportEmail, false, userId, cancellationToken);

			if (request.CompanyName != null) // Allow empty string to clear company name
				await SavePlatformSetting("CompanyName", request.CompanyName, false, userId, cancellationToken);

			// 3. Save all changes
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}

		private async Task SavePlatformSetting(string key, string value, bool isSecret, Guid userId, CancellationToken cancellationToken)
		{
			var setting = new PlatformSetting
			{
				SettingKey = key,
				SettingValue = value,
				IsSecret = isSecret
			};
			setting.CreatedAt = DateTime.UtcNow;
			setting.CreatedBy = userId;

			await _platformRepository.SaveAsync(setting, cancellationToken);
		}

		public async Task UpdatePlatformApiSettingsAsync(Guid userId, UpdatePlatformApiSettingsDto request, CancellationToken cancellationToken)
		{
			var validationResult = await _updatePlatformApiSettingsDto.ValidateAsync(request, cancellationToken);
			if (!validationResult.IsValid)
			{
				throw new AppValidationException(validationResult.Errors.Select(e => e.ErrorMessage));
			}
			var user = await _authRepository.FindByIdAsync(userId, cancellationToken);
			if (user == null)
			{
				throw new NotFoundException("User not found ");
			}
			if (user.Status != UserStatus.Active)
			{
				throw new UnauthorizedException("account is inactive.");
			}

			foreach (var setting in request.Settings)
			{
				if (!string.IsNullOrWhiteSpace(setting.Key))
				{
					// API settings are marked as IsSecret = true
					await SavePlatformSetting(setting.Key, setting.Value, true, userId, cancellationToken);
				}
			}

			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
	}
}
