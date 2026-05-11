using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.Auth
{
	public class VerifyOtpRequestDto
	{
		public string Email { get; set; } = string.Empty;
		public string Code { get; set; } = string.Empty;
		public ChannelType Channel { get; set; }
	}
}
