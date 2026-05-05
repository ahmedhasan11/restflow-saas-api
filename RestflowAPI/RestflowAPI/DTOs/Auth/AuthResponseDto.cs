namespace RestflowAPI.DTOs.Auth
{
	public class AuthResponseDto
	{
		public bool IsSuccess { get; set; }
		public string? Message { get; set; }
		public string? Token { get; set; }
		public string? RefreshToken { get; set; }
		public DateTime? ExpiresAt { get; set; }
		public IEnumerable<string>? Errors { get; set; }

		public static AuthResponseDto Success(string? message= null ,string? token = null, string? refreshToken = null, DateTime? expiresAt = null)
		{
			return new AuthResponseDto
			{
				IsSuccess = true,
				Message = message,
				Token = token,
				RefreshToken = refreshToken,
				ExpiresAt = expiresAt
			};
		}

		public static AuthResponseDto Failure(IEnumerable<string> errors)
		{
			return new AuthResponseDto
			{
				IsSuccess = false,
				Errors = errors
			};
		}

		public static AuthResponseDto Failure(string error)
		{
			return new AuthResponseDto
			{
				IsSuccess = false,
				Errors = new List<string> { error }
			};
		}
	}
}
