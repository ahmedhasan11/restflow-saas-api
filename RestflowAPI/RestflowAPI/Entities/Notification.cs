using RestflowAPI.Enums;
using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Entities
{
	public class Notification : BaseEntity, IMustHaveTenant
	{
		public Guid Id { get; set; }
		public Guid TenantId { get; set; }
		public Guid UserId { get; set; }
		public NotificationType Type { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Body { get; set; } = string.Empty;
		public DateTime? ReadAt { get; set; }

		// Navigation Properties
		public ApplicationUser? User { get; set; }
	}
}
