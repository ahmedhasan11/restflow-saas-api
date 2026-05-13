using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.Tenants
{
	public class UpdateTenantRequestDto
	{
		public string? RestaurantName { get; set; }
		public TenantStatus? Status { get; set; }
		public string? Country { get; set; }
		public string? DefaultLanguage { get; set; }
		public string? Timezone { get; set; }
		public string? Currency { get; set; }
	}
}
