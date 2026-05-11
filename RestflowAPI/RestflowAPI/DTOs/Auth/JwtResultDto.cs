namespace RestflowAPI.DTOs.Auth
{
	public class JwtResultDto
	{
		public string Token { get; set; } = string.Empty;

		public DateTime ExpiresAt { get; set; }
	}
}
