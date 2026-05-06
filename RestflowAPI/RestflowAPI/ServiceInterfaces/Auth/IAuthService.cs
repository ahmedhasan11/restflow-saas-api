using Microsoft.AspNetCore.Identity.Data;
using RestflowAPI.DTOs.Auth;

namespace RestflowAPI.ServiceInterfaces.Auth
{
	public interface IAuthService
	{
		Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);
		Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpRequestDto request, CancellationToken cancellationToken);

		Task<AuthResponseDto> ResendOtpAsync(ResendOtpRequestDto request, CancellationToken cancellationToken);

		Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);

		Task<AuthResponseDto> RefreshSessionAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken);

		Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken);

		Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken);

		Task<AuthResponseDto> LogoutAsync(LogoutRequestDto request, CancellationToken cancellationToken);
	}
}
