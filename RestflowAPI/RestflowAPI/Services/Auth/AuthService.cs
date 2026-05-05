using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;
using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.Auth;
using RestflowAPI.Entities;
using RestflowAPI.Enums;
using RestflowAPI.RepositoryInterfaces.Auth;
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
		private readonly IValidator<RegisterRequestDto> _registerValidator;
		private readonly IValidator<VerifyOtpRequestDto> _verifyOtpValidator;
		private readonly IValidator<ResendOtpRequestDto> _resendOtpValidator;
		private readonly IValidator<LoginRequestDto> _loginValidator;
		private readonly ILogger<AuthService> _logger;
		private readonly IUnitOfWork _unitOfWork;
		public AuthService(IAuthRepository authRepository, ILogger<AuthService> logger, 
			IValidator<RegisterRequestDto> registerValidator, IUnitOfWork unitOfWork, 
			IValidator<VerifyOtpRequestDto> verifyOtpValidator, IValidator<ResendOtpRequestDto>
			resendOtpValidator, IValidator<LoginRequestDto> loginValidator, IRefreshTokenService refreshTokenService
			, IJwtService jwtService, IRefreshTokenRepository refreshTokenRepository)
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
		}
		public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
		{
			//new Fluent Validation strategy
			var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);
			if (!validationResult.IsValid)
			{
				return AuthResponseDto.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
			}
			//validate if email registered
			var existingEmail = await _authRepository.FindByEmailAsync(request.Email, cancellationToken);
			if (existingEmail != null)
			{
				return AuthResponseDto.Failure("Email is already in use.");
			}
			//validate if phone registered
			var existingPhone = await _authRepository.FindByPhoneAsync(request.PhoneNumber, cancellationToken);
			if (existingPhone != null)
			{
				return AuthResponseDto.Failure("Phone number is already in use.");
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
				_logger.LogWarning("User creation failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
				return AuthResponseDto.Failure(result.Errors.Select(e => e.Description));
			}

			//assign role
			var roleResult = await _authRepository.AddToRoleAsync(user, UserRole.Owner.ToString());
			if (!roleResult.Succeeded)
			{
				_logger.LogWarning("Role assignment failed: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
				return AuthResponseDto.Failure(roleResult.Errors.Select(e => e.Description));
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
				return AuthResponseDto.Failure(result.Errors.Select(e => e.ErrorMessage));
			}
			var user = await _authRepository.FindByEmailAsync(request.Email, cancellationToken);
			if (user==null)
			{
				return AuthResponseDto.Failure("User not found.");
			}
			var otp = await _authRepository.GetLatestOtpAsync(user.Id, request.Channel, cancellationToken);
			if (otp == null)
			{
				return AuthResponseDto.Failure("No OTP found for the specified channel.");
			}
			if (otp.OtpCodeHash != HashOtp(request.Code))
			{
				return AuthResponseDto.Failure("Invalid OTP code.");
			}
			if (otp.ExpiresAt < DateTime.UtcNow)
			{
				return AuthResponseDto.Failure("OTP has expired.");
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
			await _authRepository.UpdateOtpStatusAsync(otp, cancellationToken);
			if (user.EmailConfirmed && user.PhoneNumberConfirmed)
			{
				user.Status = UserStatus.Active;
			}
			var updateResult = await _authRepository.UpdateUserAsync(user);
			if (!updateResult.Succeeded)
			{
				return AuthResponseDto.Failure(updateResult.Errors.Select(e => e.Description));
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
				return AuthResponseDto.Failure(result.Errors.Select(e => e.ErrorMessage));
			}
			var user = await _authRepository.FindByEmailAsync(request.Email, cancellationToken);
			if (user == null)
			{
				return AuthResponseDto.Failure("User not found.");
			}

			if (request.Channel == ChannelType.Email && user.EmailConfirmed)
				return AuthResponseDto.Failure("Email is already verified.");

			if (request.Channel == ChannelType.Phone && user.PhoneNumberConfirmed)
				return AuthResponseDto.Failure("Phone is already verified.");

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
				return AuthResponseDto.Failure(result.Errors.Select(e => e.ErrorMessage));
			}	
			var user = await _authRepository.FindByEmailAsync(request.Email, cancellationToken);
			if (user == null) 
			{
				return AuthResponseDto.Failure("User not found.");
			}
			var passwordValid = await _authRepository.CheckPasswordAsync(user, request.Password);
			if (passwordValid == false)
			{
				return AuthResponseDto.Failure("Email OR Password is Invalid.");
			}

			if (user.Status != UserStatus.Active)
			{
				return AuthResponseDto.Failure("User account is not active.Please verify your email and phone.");
			}

			var roles = await _authRepository.GetUserRolesAsync(user);

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
	}
}
