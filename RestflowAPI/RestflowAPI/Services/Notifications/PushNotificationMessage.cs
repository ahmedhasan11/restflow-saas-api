namespace RestflowAPI.Services.Notifications
{
	public class PushNotificationMessage
	{
		public Guid NotificationId { get; set; }
		public Guid UserId { get; set; }
		public Guid TenantId { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Body { get; set; } = string.Empty;
	}
}
