namespace RestflowAPI.DTOs.Settings
{
	public class PlatformSettingsDto
	{
		public string SystemName { get; set; } = string.Empty;
		public string? SystemLogoUrl { get; set; }
		public string DefaultLanguage { get; set; } = "en";
		public string SupportEmail { get; set; } = string.Empty;
		public string? CompanyName { get; set; }
		public Dictionary<string, string> ApiConfigurations { get; set; } = new();
	}
}
