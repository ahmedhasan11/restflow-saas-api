using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.Tenants
{
	public class TenantResponseDto
	{
		public Guid Id { get; set; }
		public string RestaurantName { get; set; } = string.Empty;
		public string TenantCode { get; set; } = string.Empty;
		public TenantStatus Status { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
