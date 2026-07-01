using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.Notifications
{
	public class RegisterDeviceTokenDto
	{
		public string Token { get; set; } = string.Empty;
		public DeviceType DeviceType { get; set; }
	}
}
