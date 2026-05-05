namespace RestflowAPI.DTOs.Auth
{
	public class ResetPasswordRequestDto
	{
		public string Identifier { get; set; } = string.Empty;
		public string OtpCode { get; set; } = string.Empty;
		public string NewPassword { get; set; } = string.Empty;
	}
}
