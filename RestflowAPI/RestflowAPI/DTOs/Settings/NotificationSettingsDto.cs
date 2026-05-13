namespace RestflowAPI.DTOs.Settings
{
	public class NotificationSettingsDto
	{
		public bool EmailNotifications { get; set; } = true;
		public bool InAppNotifications { get; set; } = true;
		public bool AiInsightsNotifications { get; set; } = false;
		public bool InventoryAlerts { get; set; } = true;
		public bool ImportantAlerts { get; set; } = true;
	}
}
