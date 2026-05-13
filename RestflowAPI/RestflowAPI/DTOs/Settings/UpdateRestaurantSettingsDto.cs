namespace RestflowAPI.DTOs.Settings
{
	public class UpdateRestaurantSettingsDto
	{
		public string? RestaurantName { get; set; }
		public string? CuisineType { get; set; }
		public string? Country { get; set; }
		public string? DefaultLanguage { get; set; }
		public string? Timezone { get; set; }
		public string? Currency { get; set; }
	}
}
