namespace RestflowAPI.ServiceInterfaces.Auth
{
	public interface ISmsService
	{
		Task SendSmsAsync(string toPhone, string message);
	}
}
