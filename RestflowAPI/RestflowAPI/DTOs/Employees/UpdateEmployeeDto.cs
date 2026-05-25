using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.Employees
{
	public class UpdateEmployeeDto
	{
		public string? FullName { get; set; }
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Role { get; set; }
		public UserStatus? Status { get; set; }
	}
}
