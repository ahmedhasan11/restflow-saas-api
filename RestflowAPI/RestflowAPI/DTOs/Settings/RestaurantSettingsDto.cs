namespace RestflowAPI.DTOs.Settings
{
	public class RestaurantSettingsDto
	{
		public string RestaurantName { get; set; } = string.Empty;
		public string? RestaurantLogoUrl { get; set; }
		public string? CuisineType { get; set; }
	}
}
