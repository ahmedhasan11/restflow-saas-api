using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;
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
		private readonly IValidator<RegisterRequestDto> _registerValidator;
		private readonly ILogger<AuthService> _logger;
		public AuthService(IAuthRepository authRepository, ILogger<AuthService> logger, IValidator<RegisterRequestDto> registerValidator)
		{
			_authRepository = authRepository;
			_logger = logger;
			_registerValidator = registerValidator;
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
			return AuthResponseDto.Success("User registered successfully.");
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
	}
}
