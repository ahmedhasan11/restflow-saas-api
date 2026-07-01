using RestflowAPI.DTOs.Notifications;
using RestflowAPI.Entities;

namespace RestflowAPI.Repository.Interfaces.Notifications
{
	public interface INotificationsRepository
	{
		Task AddAsync(Notification notification, CancellationToken ct);
		Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken ct);
		Task<List<NotificationDto>> GetByUserIdAsync(Guid userId, Guid tenantId, int page, int pageSize, CancellationToken ct);
		Task<Notification?> GetByIdAsync(Guid id, Guid userId, Guid tenantId, CancellationToken ct);
		Task<int> GetUnreadCountAsync(Guid userId, Guid tenantId, CancellationToken ct);
		void Update(Notification notification);
		Task MarkAllAsReadAsync(Guid userId, Guid tenantId, CancellationToken ct);
	}
}
