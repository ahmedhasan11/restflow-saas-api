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
			if (string.IsNullOrEmpty(_smsSettings.AccountSid) ||string.IsNullOrEmpty(_smsSettings.AuthToken) ||	string.IsNullOrEmpty(_smsSettings.FromNumber))
			{
				throw new Exception("Twilio SMS settings are incomplete. Please ensure SmsSettings:AccountSid, AuthToken, and FromNumber are configured in your secrets.json or appsettings.json.");
			}

			try
			{
				// Normalize phone number to E.164 (specifically for Egypt as per validators)
				if (toPhone.StartsWith("0"))
				{
					toPhone = "+20" + toPhone.Substring(1);
				}
				else if (!toPhone.StartsWith("+"))
				{
					toPhone = "+20" + toPhone;
				}

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
				// Throw a meaningful exception but keep the original for debugging
				throw new Exception($"Failed to send SMS to {toPhone} via Twilio. Error: {ex.Message}", ex);
			}
		}
	}
}
