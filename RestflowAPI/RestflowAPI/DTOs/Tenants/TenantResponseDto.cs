using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.Tenants
{
	public class TenantResponseDto
	{
		public Guid Id { get; set; }
		public string RestaurantName { get; set; } = string.Empty;
		public string TenantCode { get; set; } = string.Empty;
		public TenantStatus Status { get; set; }
		public string Country { get; set; } = string.Empty;
		public string DefaultLanguage { get; set; } = string.Empty;
		public string Timezone { get; set; } = string.Empty;
		public string Currency { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
	}
}
