using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.Auth
{
	public class UserProfileResultDto
	{
		public Guid Id { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Phone { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;
		public Guid? TenantId { get; set; }
		public UserStatus Status { get; set; }
	}
}
