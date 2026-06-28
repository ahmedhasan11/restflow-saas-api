namespace RestflowAPI.DTOs.Notifications
{
	public class GetNotificationsRequestDto
	{
		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 10;
	}
}
