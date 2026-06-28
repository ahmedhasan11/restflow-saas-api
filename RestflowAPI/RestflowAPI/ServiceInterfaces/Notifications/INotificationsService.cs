using RestflowAPI.DTOs.Notifications;
using RestflowAPI.Entities;

namespace RestflowAPI.ServiceInterfaces.Notifications
{
	public interface INotificationsService
	{
		Task SendLowStockAlertAsync(InventoryItem item, Guid tenantId, CancellationToken ct);
		Task SendOutOfStockAlertAsync(InventoryItem item, Guid tenantId, CancellationToken ct);
		Task SendNewOrderAlertAsync(Order order, Guid tenantId, CancellationToken ct);
		Task<NotificationListResponseDto> GetUserNotificationsAsync(Guid userId, int page, int pageSize, CancellationToken ct);
		Task MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken ct);
		Task MarkAllAsReadAsync(Guid userId, CancellationToken ct);
	}
}
