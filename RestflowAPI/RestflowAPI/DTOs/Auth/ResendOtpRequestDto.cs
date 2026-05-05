using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.Auth
{
	public class ResendOtpRequestDto
	{
		public string Email { get; set; } = string.Empty;
		public ChannelType Channel { get; set; }
	}
}
