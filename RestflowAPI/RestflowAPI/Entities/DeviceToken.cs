using RestflowAPI.Enums;
using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Entities
{
	public class DeviceToken : BaseEntity, IMustHaveTenant
	{
		public Guid Id { get; set; }
		public Guid TenantId { get; set; }
		public Guid UserId { get; set; }
		public string Token { get; set; } = string.Empty;
		public DeviceType DeviceType { get; set; }

		// Navigation Properties
		public ApplicationUser? User { get; set; }
	}
}
