using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.Employees
{
	public class EmployeeDto
	{
		public Guid Id { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;
		public UserStatus Status { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
