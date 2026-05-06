using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.Tenants
{
	public class CreateTenantRequestDto
	{
		public string RestaurantName { get; set; } = string.Empty;
		public string TenantCode { get; set; } = string.Empty;
		public TenantStatus Status { get; set; } = TenantStatus.Active;
	}
}
