namespace RestflowAPI.ServiceInterfaces.Notifications
{
	public interface IFcmPushService
	{
		Task SendPushNotificationAsync(string token, string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default);
	}
}
