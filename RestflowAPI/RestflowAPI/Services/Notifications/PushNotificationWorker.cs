
using FirebaseAdmin.Messaging;
using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.Repository.Interfaces.Notifications;
using RestflowAPI.ServiceInterfaces.Notifications;
using System.Threading.Channels;

namespace RestflowAPI.Services.Notifications
{
	public class PushNotificationWorker : BackgroundService
	{
		private readonly Channel<PushNotificationMessage> _channel;
		private readonly IFcmPushService _fcmPushService;
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly ILogger<PushNotificationWorker> _logger;

		public PushNotificationWorker(Channel<PushNotificationMessage> channel
			, IFcmPushService fcmPushService, ILogger<PushNotificationWorker> logger
			, IServiceScopeFactory serviceScopeFactory)
		{
			_channel = channel;
			_fcmPushService = fcmPushService;
			_serviceScopeFactory = serviceScopeFactory;
			_logger = logger;
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("PushNotificationWorker background service started.");

			while (await _channel.Reader.WaitToReadAsync(stoppingToken))
			{
				while (_channel.Reader.TryRead(out var message))
				{
					try
					{
						await ProcessPushNotificationAsync(message, stoppingToken);
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error processing push notification message for User {UserId}.", message.UserId);
					}
				}
			}
			_logger.LogInformation("PushNotificationWorker background service stopping.");
		}

		private async Task ProcessPushNotificationAsync(PushNotificationMessage message, CancellationToken stoppingToken)
		{
			using var scope = _serviceScopeFactory.CreateScope();
			var deviceTokenRepository = scope.ServiceProvider.GetRequiredService<IDeviceTokenRepository>();
			var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

			var deviceTokens = await deviceTokenRepository.GetByUserIdAsync(message.UserId, message.TenantId, stoppingToken);

			if (deviceTokens == null || deviceTokens.Count == 0)
			{
				_logger.LogDebug("No active device tokens found for User {UserId}. Skipping push.", message.UserId);
				return;
			}

			foreach(var deviceToken in deviceTokens)
			{
				var tokenString = deviceToken.Token;
				int attempts = 0;
				bool isSent = false;
				while (attempts < 3 && !isSent && !stoppingToken.IsCancellationRequested)
				{
					try
					{
						var data = new Dictionary<string, string>
						{
							{ "NotificationId", message.NotificationId.ToString() },
							{ "UserId", message.UserId.ToString() },
							{ "TenantId", message.TenantId.ToString() }
						};

						await _fcmPushService.SendPushNotificationAsync(tokenString, message.Title, message.Body, data, stoppingToken);
						isSent = true;
					}
					catch (FirebaseMessagingException ex) when (ex.MessagingErrorCode == MessagingErrorCode.Unregistered || ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
					{
						_logger.LogWarning(ex, "Permanent FCM error ({ErrorCode}) for token {Token}. Removing token from database.", ex.MessagingErrorCode, tokenString);
						await deviceTokenRepository.RemoveByTokenAsync(tokenString,null,null, stoppingToken);
						await uow.SaveChangesAsync(stoppingToken);
						break; // Permanent failure: stop retrying and move to next token
					}
					catch (Exception ex)
					{
						attempts++;
						if (attempts >= 3)
						{
							_logger.LogError(ex, "Transient error occurred while sending push notification to token {Token}. Dropping request after 3 attempts.", tokenString);
						}
						else
						{
							_logger.LogWarning(ex, "Transient error occurred while sending push notification to token {Token}. Attempt {Attempt} failed. Retrying in 2 seconds...", tokenString, attempts);
							await Task.Delay(2000, stoppingToken);
						}
					}
				}
			}
		}
	}
}
