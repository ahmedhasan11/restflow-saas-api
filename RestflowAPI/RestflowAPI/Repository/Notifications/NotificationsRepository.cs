using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.DTOs.Notifications;
using RestflowAPI.Entities;
using RestflowAPI.Repository.Interfaces.Notifications;

namespace RestflowAPI.Repository.Notifications
{
	public class NotificationsRepository : INotificationsRepository
	{
		private readonly ApplicationDbContext _db;

		public NotificationsRepository(ApplicationDbContext db)
		{
			_db = db;
		}
		public async Task AddAsync(Notification notification, CancellationToken ct)
		{
			await _db.Notifications.AddAsync(notification, ct);
		}

		public async Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken ct)
		{
			await _db.Notifications.AddRangeAsync(notifications, ct);
		}

		public async Task<Notification?> GetByIdAsync(Guid id, Guid userId, Guid tenantId, CancellationToken ct)
		{
			return await _db.Notifications
				.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId && x.TenantId == tenantId, ct);
		}

		public async Task<List<NotificationDto>> GetByUserIdAsync(Guid userId, Guid tenantId, int page, int pageSize, CancellationToken ct)
		{
			return await _db.Notifications
				.Where(x => x.UserId == userId && x.TenantId == tenantId)
				.OrderByDescending(x => x.CreatedAt)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.Select(x => new NotificationDto
				{
					Id = x.Id,
					Type = x.Type,
					Title = x.Title,
					Body = x.Body,
					IsRead = x.ReadAt != null,
					CreatedAt = x.CreatedAt
				})
				.ToListAsync(ct);
		}

		public async Task<int> GetUnreadCountAsync(Guid userId, Guid tenantId, CancellationToken ct)
		{
			return await _db.Notifications
				.CountAsync(x => x.UserId == userId && x.TenantId == tenantId && x.ReadAt == null, ct);
		}

		public async Task MarkAllAsReadAsync(Guid userId, Guid tenantId, CancellationToken ct)
		{
			await _db.Notifications
				.Where(x => x.UserId == userId && x.TenantId == tenantId && x.ReadAt == null)
				.ExecuteUpdateAsync(s => s.SetProperty(n => n.ReadAt, DateTime.UtcNow), ct);
		}

		public void Update(Notification notification)
		{
			_db.Notifications.Update(notification);
		}
	}
}
