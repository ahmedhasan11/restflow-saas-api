using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.DTOs.Auth;
using RestflowAPI.ServiceInterfaces.Auth;

namespace RestflowAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _authService.RegisterAsync(request, cancellationToken);

			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}
		[HttpPost("verify-otp")]
		public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _authService.VerifyOtpAsync(request, cancellationToken);

			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}
		[HttpPost("resend-otp")]
		public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _authService.ResendOtpAsync(request, cancellationToken);

			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _authService.LoginAsync(request, cancellationToken);

			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[Authorize]
		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _authService.RefreshSessionAsync(request, cancellationToken);

			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _authService.ForgotPasswordAsync(request, cancellationToken);

			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _authService.ResetPasswordAsync(request, cancellationToken);

			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[HttpPost("logout")]
		[Authorize]
		public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request, CancellationToken cancellationToken)
		{
			var result = await _authService.LogoutAsync(request, cancellationToken);

			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[Authorize]
		[HttpGet("me")]
		public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
		{
			var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
			{
				return Unauthorized();
			}

			var result = await _authService.GetMeAsync(userId, cancellationToken);
			if (result == null)
			{
				return Unauthorized();
			}

			return Ok(result);
		}

		[HttpPost("change-password")]
		[Authorize]   // any authenticated user can call this for his own account
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request,
												CancellationToken cancellationToken)
		{
			var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
				return Unauthorized();
			var result = await _authService.ChangePasswordAsync(userId, request, cancellationToken);
			return result.IsSuccess ? Ok(result) : BadRequest(result);
		}
	}
}
