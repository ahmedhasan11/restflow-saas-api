using RestflowAPI.Enums;
using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Entities
{
	public class Employee: BaseEntity, IMustHaveTenant
	{
		public Guid Id { get; set; }
		public Guid TenantId { get; set; }
		public Guid? UserId { get; set; } // Foreign key referencing ApplicationUser (Identity)

		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;
		public UserStatus Status { get; set; } = UserStatus.Active;

		// Navigation Properties
		public Tenant Tenant { get; set; } = null!;
		public ApplicationUser? User { get; set; }
	}
}
