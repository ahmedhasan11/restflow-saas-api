namespace RestflowAPI.DTOs.Settings
{
	public class UpdatePlatformSettingsDto
	{
		public string? SystemName { get; set; }
		public string? SystemLogoUrl { get; set; }
		public string? DefaultLanguage { get; set; }
		public string? SupportEmail { get; set; }
		public string? CompanyName { get; set; }
	}
}
