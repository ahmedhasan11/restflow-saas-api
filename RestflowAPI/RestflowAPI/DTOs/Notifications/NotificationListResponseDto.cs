namespace RestflowAPI.DTOs.Notifications
{
	public class NotificationListResponseDto
	{
		public List<NotificationDto> Notifications { get; set; } = new List<NotificationDto>();
		public int UnreadCount { get; set; }
		public int TotalCount { get; set; }
	}
}
