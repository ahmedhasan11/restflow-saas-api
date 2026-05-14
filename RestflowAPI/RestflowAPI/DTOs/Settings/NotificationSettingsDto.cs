namespace RestflowAPI.DTOs.Settings
{
	public class NotificationSettingsDto
	{
		public bool? EmailNotifications { get; set; }
		public bool? InAppNotifications { get; set; }
		public bool? AiInsightsNotifications { get; set; }
		public bool? InventoryAlerts { get; set; }
		public bool? ImportantAlerts { get; set; }
	}
}
