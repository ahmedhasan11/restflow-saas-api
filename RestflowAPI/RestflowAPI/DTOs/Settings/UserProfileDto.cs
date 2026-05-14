namespace RestflowAPI.DTOs.Settings
{
	public class UserProfileDto
	{
		public string FullName { get; set; } = string.Empty;
		public string? ProfileImageUrl { get; set; }
		public string PreferredLanguage { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
	}
}
