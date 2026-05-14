namespace RestflowAPI.DTOs.Settings
{
	public class RestaurantSettingsDto
	{
		public string RestaurantName { get; set; } = string.Empty;
		public string? RestaurantLogoUrl { get; set; }
		public string? CuisineType { get; set; }
		public string Country { get; set; } = string.Empty;
		public string DefaultLanguage { get; set; } = "en";
		public string Timezone { get; set; } = "UTC";
		public string Currency { get; set; } = "USD";
	}
}
