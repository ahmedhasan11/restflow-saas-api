using Microsoft.Extensions.Options;
using RestflowAPI.ServiceInterfaces.Auth;
using RestflowAPI.Settings;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace RestflowAPI.Services.Auth
{
	public class SmsService:ISmsService
	{
		private readonly SmsSettings _smsSettings;

		public SmsService(IOptions<SmsSettings> smsSettings)
		{
			_smsSettings = smsSettings.Value;
		}

		public async Task SendSmsAsync(string toPhone, string message)
		{
			try
			{
				TwilioClient.Init(_smsSettings.AccountSid, _smsSettings.AuthToken);

				var messageOptions = new CreateMessageOptions(new PhoneNumber(toPhone))
				{
					From = new PhoneNumber(_smsSettings.FromNumber),
					Body = message
				};

				await MessageResource.CreateAsync(messageOptions);
			}
			catch (Exception ex)
			{
				// Log the exception if needed
				throw new Exception("Failed to send SMS via Twilio. Please check your credentials.", ex);
			}
		}
	}
}
