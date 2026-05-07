using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.Auth;
using RestflowAPI.Entities;
using RestflowAPI.Enums;
using RestflowAPI.RepositoryInterfaces.Auth;
using RestflowAPI.Exceptions;
using RestflowAPI.RepositoryInterfaces.Tenants;
using RestflowAPI.ServiceInterfaces.Auth;
using System.Security.Cryptography;
using System.Text;

namespace RestflowAPI.Services.Auth
{
	public class AuthService : IAuthService
	{
		private readonly IAuthRepository _authRepository;
		private readonly IRefreshTokenRepository _refreshTokenRepository;
		private readonly IRefreshTokenService _refreshTokenService;
		private readonly IJwtService _jwtService;
		private readonly ITenantRepository _tenantRepository;
		private readonly IEmailService _emailService;
		private readonly ISmsService _smsService;
		private readonly IValidator<RegisterRequestDto> _registerValidator;
		private readonly IValidator<VerifyOtpRequestDto> _verifyOtpValidator;
		private readonly IValidator<ResendOtpRequestDto> _resendOtpValidator;
		private readonly IValidator<LoginRequestDto> _loginValidator;
		private readonly IValidator<RefreshTokenRequestDto> _refreshTokenValidator;
		private readonly IValidator<ForgotPasswordRequestDto> _forgotPasswordValidator;
		private readonly IValidator<ResetPasswordRequestDto> _resetPasswordValidator;
		private readonly IValidator<LogoutRequestDto> _logoutRequestValidator;
		private readonly IValidator<CreateUserByAdminDto> _createUserByAdminValidator;
		private readonly IValidator<ChangePasswordDto> _changePasswordValidator;
		private readonly ILogger<AuthService> _logger;
		private readonly IUnitOfWork _unitOfWork;

		public AuthService(IAuthRepository authRepository, ILogger<AuthService> logger, 
			IValidator<RegisterRequestDto> registerValidator, IUnitOfWork unitOfWork,
			IValidator<VerifyOtpRequestDto> verifyOtpValidator, IValidator<ResendOtpRequestDto>
			resendOtpValidator, IValidator<LoginRequestDto> loginValidator, IRefreshTokenService refreshTokenService
			, IJwtService jwtService, IRefreshTokenRepository refreshTokenRepository
			, IValidator<RefreshTokenRequestDto> refreshTokenValidator, IValidator<ForgotPasswordRequestDto> forgotPasswordValidator
			, IValidator<ResetPasswordRequestDto> resetPasswordValidator, IValidator<LogoutRequestDto> logoutRequestValidator
			, IValidator<CreateUserByAdminDto> createUserByAdminValidator, ITenantRepository tenantRepository
			, IValidator<ChangePasswordDto> changePasswordValidator, IEmailService emailService
			, ISmsService smsService)
		{
			_authRepository = authRepository;
			_logger = logger;
			_registerValidator = registerValidator;
			_unitOfWork = unitOfWork;
			_verifyOtpValidator = verifyOtpValidator;
			_resendOtpValidator = resendOtpValidator;
			_loginValidator = loginValidator;
			_refreshTokenService = refreshTokenService;
			_jwtService = jwtService;
			_refreshTokenRepository = refreshTokenRepository;
			_refreshTokenValidator = refreshTokenValidator;
			_forgotPasswordValidator = forgotPasswordValidator;
			_resetPasswordValidator = resetPasswordValidator;
			_logoutRequestValidator = logoutRequestValidator;
			_createUserByAdminValidator = createUserByAdminValidator;
			_tenantRepository = tenantRepository;
			_changePasswordValidator = changePasswordValidator;
			_emailService = emailService;
			_smsService = smsService;
		}
		public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
		{
			//new Fluent Validation strategy
			var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);
			if (!validationResult.IsValid)
			{
				throw new AppValidationException(validationResult.Errors.Select(e=>e.ErrorMessage));
			}
			//validate if email registered
			var existingEmail = await _authRepository.FindByEmailAsync(request.Email, cancellationToken);
			if (existingEmail != null)
			{
				throw new ConflictException("Email is already in use.");
			}
			//validate if phone registered
			var existingPhone = await _authRepository.FindByPhoneAsync(request.PhoneNumber, cancellationToken);
			if (existingPhone != null)
			{
				throw new ConflictException("Phone number is already in use.");
			}

			//create user
			ApplicationUser user = new ApplicationUser
			{
				UserName = request.Email,
				Email = request.Email,
				PhoneNumber = request.PhoneNumber,
				Status = UserStatus.Inactive,
				CreatedAt = DateTime.UtcNow
			};

			//assign user
			var result = await _authRepository.CreateUserAsync(user, request.Password);
			if (!result.Succeeded)
			{
				throw new AppValidationException(result.Errors.Select(e => e.Description));
			}

			//assign role
			var roleResult = await _authRepository.AddToRoleAsync(user, UserRole.Owner.ToString());
			if (!roleResult.Succeeded)
			{
				throw new AppValidationException(roleResult.Errors.Select(e => e.Description));
			}


			//OTP generation and saving would go here in the future
			await GenerateAndSaveOtp(user.Id, ChannelType.Email, cancellationToken);
			await GenerateAndSaveOtp(user.Id, ChannelType.Phone, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return AuthResponseDto.Success("Registration successful. Please verify your email and phone.");
		}
		private async Task GenerateAndSaveOtp(Guid userId , ChannelType channel , CancellationToken cancellationToken)
		{
			var code = new Random().Next(100000, 999999).ToString(); // Simple 6-digit OTP generation

			var hashedCode = HashOtp(code);
			var otp = new OtpVerification
			{
				UserId = userId,
				ChannelType = channel,
				OtpCodeHash = hashedCode,
				ExpiresAt = DateTime.UtcNow.AddMinutes(10), // OTP valid for 10 minutes
				IsUsed = false,
				CreatedAt = DateTime.UtcNow
			};
			await _authRepository.SaveOtpAsync(otp, cancellationToken);
			// SRS 3.2 - Send OTP via Email if channel is Email
			if (channel == ChannelType.Email)
			{
				var user = await _authRepository.FindByIdAsync(userId, cancellationToken);
				if (user != null && !string.IsNullOrEmpty(user.Email))
				{
					await _emailService.SendEmailAsync(user.Email, "Restflow - Verification Code", $"Your verification code is: {code}");
				}
			}
			else if (channel == ChannelType.Phone)
			{
				var user = await _authRepository.FindByIdAsync(userId, cancellationToken);
				if (user != null && !string.IsNullOrEmpty(user.PhoneNumber))
				{
					await _smsService.SendSmsAsync(user.PhoneNumber, $"Restflow: Your verification code is {code}");
				}
			}
		}
		private string HashOtp(string code)
		{
			using var sha256 = SHA256.Create();
			var bytes = Encoding.UTF8.GetBytes(code);
			var hash = sha256.ComputeHash(bytes);
			return Convert.ToBase64String(hash);
		}
		public async Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _verifyOtpValidator.ValidateAsync(request, cancellationToken);	
			if (!result.IsValid)
			{
				throw new AppValidationException(result.Errors.Select(e => e.ErrorMessage));
			}
			var user = await _authRepository.FindByEmailAsync(request.Email, cancellationToken);
			if (user==null)
			{
				throw new NotFoundException("User not found.");
			}
			var otp = await _authRepository.GetLatestOtpAsync(user.Id, request.Channel, cancellationToken);
			if (otp == null)
			{
				throw new NotFoundException("No active OTP found.");
			}
			if (otp.OtpCodeHash != HashOtp(request.Code))
			{
				throw new AppValidationException("Invalid OTP code.");
			}
			if (otp.ExpiresAt < DateTime.UtcNow)
			{
				throw new AppValidationException("OTP code has expired.");
			}
			if (otp.ChannelType==ChannelType.Email)
			{
				user.EmailConfirmed = true;
			}
			else
			{
				user.PhoneNumberConfirmed = true;
			}

			otp.IsUsed = true;
			if (user.EmailConfirmed && user.PhoneNumberConfirmed)
			{
				user.Status = UserStatus.Active;
			}
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			var message = request.Channel == ChannelType.Email ? "Email verified." : "Phone verified.";
			if (user.Status == UserStatus.Active) message += " Account is now active.";
			return AuthResponseDto.Success(message, "OTP verified successfully.");
		}
		public async Task<AuthResponseDto> ResendOtpAsync(ResendOtpRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _resendOtpValidator.ValidateAsync(request, cancellationToken);
			if (!result.IsValid)

			{
				throw new AppValidationException(result.Errors.Select(e => e.ErrorMessage));
			}
			var user = await _authRepository.FindByEmailAsync(request.Email, cancellationToken);
			if (user == null)
			{
				throw new NotFoundException("User not found.");
			}

			if (request.Channel == ChannelType.Email && user.EmailConfirmed)
				throw new ConflictException("Email is already verified.");

			if (request.Channel == ChannelType.Phone && user.PhoneNumberConfirmed)
				throw new ConflictException("Phone is already verified.");

			var lastOtp = await _authRepository.GetLatestOtpAsync(user.Id, request.Channel, cancellationToken);
			if (lastOtp!=null && lastOtp.CreatedAt.AddMinutes(1) > DateTime.UtcNow)
			{
				throw new ConflictException("Please wait a minute before requesting another OTP.");
			}
			await _authRepository.InvalidateOldOtpsAsync(user.Id, request.Channel, cancellationToken);
			await GenerateAndSaveOtp(user.Id, request.Channel, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return AuthResponseDto.Success("OTP resent successfully.");
		}
		public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _loginValidator.ValidateAsync(request, cancellationToken);
			if (!result.IsValid)
			{
				throw new AppValidationException(result.Errors.Select(e => e.ErrorMessage));
			}	
			var user = await _authRepository.FindByIdentifierAsync(request.Email, cancellationToken);
			if (user == null) 
			{
				throw new UnauthorizedException("Invalid credentials.");
			}
			if (await _authRepository.IsLockedOutAsync(user))
			{
				throw new UnauthorizedException("Account is temporarily blocked due to repeated failed login attempts. Please try again in 15 minutes.");
			}
			var isPasswordValid = await _authRepository.CheckPasswordAsync(user, request.Password);
			if (!isPasswordValid)
			{
				await _authRepository.IncrementAccessFailedCountAsync(user);
				throw new UnauthorizedException("Invalid credentials.");
			}

			if (user.Status != UserStatus.Active)
			{
				throw new UnauthorizedException("Account is not active. Please verify your email and phone.");
			}
			// Reset failed attempts on success
			await _authRepository.ResetAccessFailedCountAsync(user);

			var roles = await _authRepository.GetUserRolesAsync(user);
			var isSuperAdmin = roles.Contains(UserRole.SuperAdmin.ToString());
			if (user.TenantId!=null && !isSuperAdmin)
			{
				if (user.Tenant?.Status==TenantStatus.Inactive)
				{
					throw new UnauthorizedException("Your restaurant account is inactive. Please contact support.");
				}
			}
			JwtUserDataDto jwtUserData = new JwtUserDataDto
			{
				UserId = user.Id,
				Email = user.Email!,
				FullName = user.FullName,
				TenantId = user.TenantId,
				Roles = roles
			};
			var jwtResult = await _jwtService.GenerateTokenAsync(jwtUserData);

			var rawRefreshToken= _refreshTokenService.GenerateRefreshToken();
			var hashedRefreshToken =  HashOtp(rawRefreshToken);

			RefreshToken refreshToken = new RefreshToken
			{
				UserId = user.Id,
				TokenHash = hashedRefreshToken,
				IssuedAt = DateTime.UtcNow,
				ExpiresAt = DateTime.UtcNow.AddDays(7), 
				IsRevoked = false
			};

			await _refreshTokenRepository.SaveRefreshTokenAsync(refreshToken, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return AuthResponseDto.Success("Login successful.", jwtResult.Token, rawRefreshToken, jwtResult.ExpiresAt);

		}
		public async Task<AuthResponseDto> RefreshSessionAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _refreshTokenValidator.ValidateAsync(request, cancellationToken);	
			if (!result.IsValid)
			{
				throw new AppValidationException(result.Errors.Select(e => e.ErrorMessage));
			}	

			var recievedTokenHash = HashOtp(request.RefreshToken);
			var storedToken = await _refreshTokenRepository.GetRefreshTokenAsync(recievedTokenHash, cancellationToken);
			if (storedToken==null || storedToken.IsRevoked || storedToken.ExpiresAt<DateTime.UtcNow)
			{
				throw new UnauthorizedException("Invalid or expired refresh token.");
			}
			var user = storedToken.User;
			if (user == null)
			{
				if (user == null) throw new Exceptions.UnauthorizedException("User session invalid.");
			}
			var roles = await _authRepository.GetUserRolesAsync(user);
			
			var isSuperAdmin = roles.Contains(UserRole.SuperAdmin.ToString());
			if (user.TenantId != null && !isSuperAdmin)
			{
				if (user.Tenant?.Status == TenantStatus.Inactive)
				{
					throw new Exceptions.UnauthorizedException("Your restaurant account is inactive.");
				}
			}

			var jwtResult = await _jwtService.GenerateTokenAsync(new JwtUserDataDto
			{
				UserId = user.Id,
				Email = user.Email!,
				FullName = user.FullName,
				TenantId = user.TenantId,
				Roles = roles
			});

			var newRawRefreshToken = _refreshTokenService.GenerateRefreshToken();
			var newHashedRefreshToken = HashOtp(newRawRefreshToken);

			storedToken.IsRevoked = true;

			var newRefreshToken = new RefreshToken
			{
				UserId = user.Id,
				TokenHash = newHashedRefreshToken,
				IssuedAt = DateTime.UtcNow,
				ExpiresAt = DateTime.UtcNow.AddDays(7),
				IsRevoked = false
			};

			await _refreshTokenRepository.SaveRefreshTokenAsync(newRefreshToken, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return AuthResponseDto.Success("Session refreshed successfully.", jwtResult.Token, newRawRefreshToken, jwtResult.ExpiresAt);

		}
		public async Task<AuthResponseDto> ChangePasswordAsync(Guid userId, ChangePasswordDto request, CancellationToken cancellationToken)
		{
			// 1️⃣ Validate DTO
			var validation = await _changePasswordValidator.ValidateAsync(request, cancellationToken);
			if (!validation.IsValid)
					throw new AppValidationException(validation.Errors.Select(e => e.ErrorMessage));

			// 2️⃣ Load user
			var user = await _authRepository.FindByIdAsync(userId, cancellationToken);
			if (user == null || user.Status != UserStatus.Active)
				throw new NotFoundException("User not found or inactive.");

			// 4️⃣ Change password (Identity handles hashing & persistence)
			var result = await _authRepository.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
			if (!result.Succeeded)
				throw new UnauthorizedException("Invalid current password or password policy violation.");

			// 5️⃣ Optional: revoke all refresh tokens so the user must re‑login
			await _refreshTokenRepository.RevokeAllUserRefreshTokensAsync(user.Id, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return AuthResponseDto.Success("Password changed successfully.");
		}
		public async Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _forgotPasswordValidator.ValidateAsync(request, cancellationToken);
			if (!result.IsValid)
			{
				throw new Exceptions.AppValidationException(result.Errors.Select(e => e.ErrorMessage));
			}
			var user = await _authRepository.FindByIdentifierAsync(request.Identifier, cancellationToken);

			// SRS 3.6 - Generic response for security
			if (user != null && user.Status == UserStatus.Active)
			{
				ChannelType channel = request.Identifier.Contains("@") ? ChannelType.Email : ChannelType.Phone;

				// SRS 3.2 - Check resend interval (1 minute)
				var lastOtp = await _authRepository.GetLatestOtpAsync(user.Id, channel, cancellationToken);
				if (lastOtp != null && lastOtp.CreatedAt.AddMinutes(1) > DateTime.UtcNow)
				{
					throw new Exceptions.ConflictException("Please wait a minute before requesting another reset code.");
				}

				await _authRepository.InvalidateOldOtpsAsync(user.Id, channel, cancellationToken);
				await GenerateAndSaveOtp(user.Id, channel, cancellationToken);
				await _unitOfWork.SaveChangesAsync(cancellationToken);
			}
			return AuthResponseDto.Success("If an account exists, a reset code has been sent.");
		}
		public async Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _resetPasswordValidator.ValidateAsync(request, cancellationToken);
			if (!result.IsValid)
			{
				throw new Exceptions.AppValidationException(result.Errors.Select(e => e.ErrorMessage));
			}

			var user = await _authRepository.FindByIdentifierAsync(request.Identifier, cancellationToken);
			if (user == null)
			{
				throw new Exceptions.NotFoundException("User not found.");
			}

			// Security check: ensure account is active during reset
			if (user.Status != UserStatus.Active)
			{
				throw new Exceptions.AppValidationException("Account is not active.");
			}
			ChannelType channel = request.Identifier.Contains("@") ? ChannelType.Email : ChannelType.Phone;

			var otp = await _authRepository.GetLatestOtpAsync(user.Id, channel, cancellationToken);
			if (otp == null || otp.IsUsed || otp.ExpiresAt < DateTime.UtcNow || otp.OtpCodeHash != HashOtp(request.OtpCode))
			{
				throw new Exceptions.AppValidationException("Invalid or expired reset code.");
			}

			var resetResult = await _authRepository.ResetPasswordAsync(user, request.NewPassword);
			if (!resetResult.Succeeded)
			{
				throw new Exceptions.AppValidationException(resetResult.Errors.Select(e => e.Description));
			}

			otp.IsUsed = true;

			// Revoke all active sessions (Security requirement)
			await _refreshTokenRepository.RevokeAllUserRefreshTokensAsync(user.Id, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return AuthResponseDto.Success("Password reset successfully.");
		}
		public async Task<AuthResponseDto> LogoutAsync(LogoutRequestDto request, CancellationToken cancellationToken)
		{
		   var result = await _logoutRequestValidator.ValidateAsync(request, cancellationToken);
			if (!result.IsValid)
			{
				throw new Exceptions.AppValidationException(result.Errors.Select(e => e.ErrorMessage));
			}
			var refreshTokenHash = HashOtp(request.RefreshToken);
			var storedToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshTokenHash, cancellationToken);
			if(storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow )
			{
				throw new Exceptions.UnauthorizedException("Invalid or expired refresh token.");
			}
			await _refreshTokenRepository.RevokeAllUserRefreshTokensAsync(storedToken.UserId, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return AuthResponseDto.Success("Logged out successfully.");

		}
		public async Task<UserProfileResultDto?> GetMeAsync(Guid userId, CancellationToken cancellationToken)
		{
			if (userId==Guid.Empty)
			{
				return null;
			}
			var user = await _authRepository.FindByIdAsync(userId, cancellationToken);
			if (user == null || user.Status != UserStatus.Active)
			{
				throw new Exceptions.UnauthorizedException("User not found or inactive.");
			}
			var roles = await _authRepository.GetUserRolesAsync(user);

			return new UserProfileResultDto
			{
				Id = user.Id,
				FullName = user.FullName,
				Email = user.Email ?? string.Empty,
				Phone = user.PhoneNumber ?? string.Empty,
				Role = roles.FirstOrDefault() ?? string.Empty,
				TenantId = user.TenantId,
				Status = user.Status
			};
		}
		public async Task<AuthResponseDto> CreateUserByAdminAsync(CreateUserByAdminDto request, CancellationToken cancellationToken)
		{
			var result = await _createUserByAdminValidator.ValidateAsync(request, cancellationToken);
			if (!result.IsValid)
			{
				throw new Exceptions.AppValidationException(result.Errors.Select(e => e.ErrorMessage));
			}

			// Verify Tenant Existence (US-17)
			var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken);
			if (tenant == null)
			{
				throw new Exceptions.NotFoundException("The specified restaurant (Tenant) does not exist.");
			}

			var existingEmail = await _authRepository.FindByEmailAsync(request.Email, cancellationToken);
			if (existingEmail != null)
				throw new Exceptions.ConflictException("Email is already in use.");

			var existingPhone = await _authRepository.FindByPhoneAsync(request.PhoneNumber, cancellationToken);
			if (existingPhone != null)
				throw new Exceptions.ConflictException("Phone number is already in use.");


			ApplicationUser user = new ApplicationUser
			{
				UserName = request.Email,
				Email = request.Email,
				PhoneNumber = request.PhoneNumber,
				FullName = request.FullName,
				TenantId = request.TenantId,
				Status = UserStatus.Active,
				EmailConfirmed = true,
				PhoneNumberConfirmed = true,
				CreatedAt = DateTime.UtcNow
			};

			var createResult = await _authRepository.CreateUserAsync(user, request.Password);
			if (!createResult.Succeeded)
			{
				throw new Exceptions.AppValidationException(createResult.Errors.Select(e => e.Description));
			}
			await _authRepository.AddToRoleAsync(user, request.Role.ToString());
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return AuthResponseDto.Success("User created successfully by admin.");
		}

	}
}
