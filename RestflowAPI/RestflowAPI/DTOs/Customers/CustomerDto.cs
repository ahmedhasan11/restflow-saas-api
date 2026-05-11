using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.Customers
{
	public class CustomerDto
	{
		public Guid Id { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public CustomerStatus Status { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}
}
