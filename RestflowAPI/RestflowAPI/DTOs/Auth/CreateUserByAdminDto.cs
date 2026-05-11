using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.Auth
{
	public class CreateUserByAdminDto
	{
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public UserRole Role { get; set; } // Owner or Employee
		public Guid TenantId { get; set; }
	}
}
