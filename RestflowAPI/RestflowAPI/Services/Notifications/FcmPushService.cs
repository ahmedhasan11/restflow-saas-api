using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using RestflowAPI.ServiceInterfaces.Notifications;
using System.Text.Json;

namespace RestflowAPI.Services.Notifications
{
	public class FcmPushService : IFcmPushService
	{
		private readonly ILogger<FcmPushService> _logger;
		private readonly bool _isInitialized;
		
		public FcmPushService(IConfiguration configuration, ILogger<FcmPushService> logger)
		{
			_logger = logger;
			try
			{
				if (FirebaseApp.DefaultInstance == null)
				{
					var serviceAccountDict = configuration.GetSection("Firebase:ServiceAccountKey")
						.GetChildren()
						.ToDictionary(x => x.Key, x => x.Value);
					if (serviceAccountDict != null && serviceAccountDict.Any(x => !string.IsNullOrEmpty(x.Value)))
					{
						var jsonCredentials = JsonSerializer.Serialize(serviceAccountDict);
						FirebaseApp.Create(new AppOptions
						{
							Credential = CredentialFactory.FromJson<ServiceAccountCredential>(jsonCredentials).ToGoogleCredential()
						});
						_logger.LogInformation("FirebaseApp successfully initialized for FCM.");
						_isInitialized = true;
					}
					else
					{
						_logger.LogWarning("Firebase ServiceAccountKey configuration is missing or empty. Push notifications will be skipped.");
						_isInitialized = false;
					}
				}
				else
				{
					_isInitialized = true;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to initialize FirebaseApp.");
				_isInitialized = false;
			}
		}
		public async Task SendPushNotificationAsync(string token, string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default)
		{
			if (!_isInitialized)
			{
				_logger.LogWarning("FCM Service is not initialized. Skipping push notification to token: {Token}", token);
				return;
			}
			var message = new Message
			{
				Token = token,
				Notification = new FirebaseAdmin.Messaging.Notification
				{
					Title = title,
					Body = body
				},
				Data = data
			};
			await FirebaseMessaging.DefaultInstance.SendAsync(message, ct);
			_logger.LogInformation("Successfully sent push notification to token: {Token}", token);
		}
	}
}
