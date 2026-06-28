using Microsoft.EntityFrameworkCore;
using RestflowAPI.Constants;
using RestflowAPI.Data;
using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.Notifications;
using RestflowAPI.DTOs.Settings;
using RestflowAPI.Entities;
using RestflowAPI.Enums;
using RestflowAPI.Exceptions;
using RestflowAPI.Repository.Interfaces.Notifications;
using RestflowAPI.Repository.Notifications;
using RestflowAPI.ServiceInterfaces.Notifications;
using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Services.Notifications
{
	public class NotificationsService : INotificationsService
	{
		private readonly INotificationsRepository _notificationsRepository;
		private readonly ApplicationDbContext _db;
		private readonly ICurrentTenantService _tenantService;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<NotificationsService> _logger;
		public NotificationsService(INotificationsRepository notificationsRepository, ApplicationDbContext db,
			ICurrentTenantService tenantService, IUnitOfWork unitOfWork, ILogger<NotificationsService> logger)
		{
			_notificationsRepository = notificationsRepository;
			_db = db;
			_tenantService = tenantService;
			_unitOfWork = unitOfWork;
			_logger = logger;
		}
		public async Task<NotificationListResponseDto> GetUserNotificationsAsync(Guid userId, int page, int pageSize, CancellationToken ct)
		{
			var tenantId = _tenantService.TenantId ?? throw new Exception("Tenant required");

			var notifications = await _notificationsRepository.GetByUserIdAsync(userId, tenantId, page, pageSize, ct);
			var unreadCount = await _notificationsRepository.GetUnreadCountAsync(userId, tenantId, ct);

			var totalCount = await _db.Notifications.CountAsync(x => x.UserId == userId && x.TenantId == tenantId, ct);


			return new NotificationListResponseDto
			{
				Notifications = notifications,
				UnreadCount = unreadCount,
				TotalCount = totalCount
			};
		}
		public async Task MarkAllAsReadAsync(Guid userId, CancellationToken ct)
		{
			var tenantId = _tenantService.TenantId ?? throw new UnauthorizedException("Tenant required");
			await _notificationsRepository.MarkAllAsReadAsync(userId, tenantId, ct);
		}
		public async Task MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken ct)
		{
			var tenantId = _tenantService.TenantId ?? throw new UnauthorizedException("Tenant required");
			var notification = await _notificationsRepository.GetByIdAsync(notificationId, userId, tenantId, ct);
			if (notification == null)
			{
				throw new NotFoundException("Notification not found");
			}

			if (notification.ReadAt == null)
			{
				notification.ReadAt = DateTime.UtcNow;
				_notificationsRepository.Update(notification);
				await _unitOfWork.SaveChangesAsync(ct);
			}
		}
		public async Task SendLowStockAlertAsync(InventoryItem item, Guid tenantId, CancellationToken ct)
		{

			// Title & Body in Arabic as per SRS
			var title = "تنبيه: مخزون منخفض";
			var body = $"تنبيه: مادة {item.ItemName} وصلت للحد الأدنى في المخزون. الكمية الحالية: {item.CurrentQuantity} {item.UnitOfMeasure}.";

			await CreateAndSaveNotificationsForAudienceAsync(tenantId,	NotificationType.LowStock,	title, body,
					"InventoryAlerts",	isOperationalOnly: false, ct);
		}
		public async Task SendNewOrderAlertAsync(Order order, Guid tenantId, CancellationToken ct)
		{
			// Title & Body in Arabic as per SRS
			var title = "طلب جديد";
			var body = $"طلب جديد رقم #{order.OrderNumber} مضاف حالياً بحالة معلق.";

			await CreateAndSaveNotificationsForAudienceAsync(tenantId, NotificationType.NewOrder, title, body,
					"ImportantAlerts", isOperationalOnly: true, ct);
		}
		public async Task SendOutOfStockAlertAsync(InventoryItem item, Guid tenantId, CancellationToken ct)
		{
			// Title & Body in Arabic as per SRS
			var title = "تنبيه: نفاد المخزون";
			var body = $"عاجل: نفدت مادة {item.ItemName} تماماً من المخزون! المنتجات المرتبطة بها ستتأثر.";

			await CreateAndSaveNotificationsForAudienceAsync(tenantId, NotificationType.OutOfStock, title, body,
					"InventoryAlerts", isOperationalOnly: false, ct);
		}
		private async Task CreateAndSaveNotificationsForAudienceAsync(Guid tenantId, NotificationType type,
		string title, string body, string toggleName, bool isOperationalOnly, CancellationToken ct)
		{
			if (tenantId == Guid.Empty)
			{
				throw new UnauthorizedException("Tenant required");
			}
			// Get all active users in the tenant
			var activeUsers = await _db.Users
				.Where(u => u.TenantId == tenantId && u.Status == UserStatus.Active)
				.ToListAsync(ct);

			if (!activeUsers.Any())
				return;

			// Fetch owner role ID and the user IDs that have the Owner role
			var ownerRole = await _db.Roles
				.FirstOrDefaultAsync(r => r.Name == Permissions.Roles.Owner, ct);

			var ownerUserIds = ownerRole != null
				? await _db.UserRoles
					.Where(ur => ur.RoleId == ownerRole.Id)
					.Select(ur => ur.UserId)
					.ToListAsync(ct)
				: new List<Guid>();

			// Get active employees of the tenant to check operational roles if isOperationalOnly is true
			var activeEmployees = await _db.Employees
				.Where(e => e.TenantId == tenantId && e.Status == UserStatus.Active && e.UserId != null)
				.ToDictionaryAsync(e => e.UserId!.Value, e => e.Role, ct);

			var notificationsToCreate = new List<Notification>();

			foreach (var user in activeUsers)
			{
				// If it is filtered by operational roles, we check if they are employees with operational roles OR if they are the owner
				if (isOperationalOnly)
				{
					var isOwner = ownerUserIds.Contains(user.Id);

					if (!isOwner)
					{
						if (activeEmployees.TryGetValue(user.Id, out var role))
						{
							var hasOperationalRole = role.Equals(Permissions.Roles.Employee, StringComparison.OrdinalIgnoreCase) ||
													 role.Equals(Permissions.Roles.KitchenStaff, StringComparison.OrdinalIgnoreCase) ||
													 role.Equals(Permissions.Roles.Cashier, StringComparison.OrdinalIgnoreCase) ||
													 role.Equals(Permissions.Roles.Manager, StringComparison.OrdinalIgnoreCase);

							if (!hasOperationalRole)
								continue;
						}
						else
						{
							// If not the owner and not in active employees, exclude from operational notifications
							continue;
						}
					}
				}

				// Check user preferences
				if (!IsNotificationEnabled(user, toggleName))
					continue;

				notificationsToCreate.Add(new Notification
				{
					Id = Guid.NewGuid(),
					TenantId = tenantId,
					UserId = user.Id,
					Type = type,
					Title = title,
					Body = body,
					ReadAt = null
				});
			}

			if (notificationsToCreate.Any())
			{
				await _notificationsRepository.AddRangeAsync(notificationsToCreate, ct);
				await _unitOfWork.SaveChangesAsync(ct);
			}
		}
		private bool IsNotificationEnabled(ApplicationUser user, string toggleName)
		{
			var inAppNotifications = true;
			var categoryNotifications = true;

			if (!string.IsNullOrEmpty(user.NotificationPreferences))
			{
				try
				{
					var savedSettings = System.Text.Json.JsonSerializer.Deserialize<NotificationSettingsDto>(user.NotificationPreferences);
					if (savedSettings != null)
					{
						inAppNotifications = savedSettings.InAppNotifications ?? true;

						if (toggleName == "InventoryAlerts")
						{
							categoryNotifications = savedSettings.InventoryAlerts ?? true;
						}
						else if (toggleName == "ImportantAlerts")
						{
							categoryNotifications = savedSettings.ImportantAlerts ?? true;
						}
					}
				}
				catch(Exception ex)
				{
					_logger.LogWarning(ex, "Failed to deserialize notification preferences for user {UserId}. Falling back to default settings.", user.Id);
				}
			}
			return inAppNotifications && categoryNotifications;
		}
	}
}
