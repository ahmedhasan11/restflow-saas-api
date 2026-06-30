using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.Entities;
using RestflowAPI.Repository.Interfaces.Notifications;

namespace RestflowAPI.Repository.Notifications
{
	public class DeviceTokenRepository: IDeviceTokenRepository
	{
		private readonly ApplicationDbContext _db;

		public DeviceTokenRepository(ApplicationDbContext db)
		{
			_db = db;
		}
		public async Task UpsertAsync(DeviceToken token, CancellationToken ct)
		{
			// Use IgnoreQueryFilters to ensure we locate any existing token, even if soft-deleted or assigned to another tenant
			var existing = await _db.DeviceTokens
				.IgnoreQueryFilters()
				.FirstOrDefaultAsync(x => x.Token == token.Token, ct);

			if (existing != null)
			{
				existing.UserId = token.UserId;
				existing.TenantId = token.TenantId;
				existing.DeviceType = token.DeviceType;
				existing.DeletedAt = null; // Un-delete if it was soft-deleted
			}
			else
			{
				token.Id = Guid.NewGuid();
				await _db.DeviceTokens.AddAsync(token, ct);
			}
		}
		public async Task<List<DeviceToken>> GetByUserIdAsync(Guid userId, Guid tenantId, CancellationToken ct)
		{
			return await _db.DeviceTokens
				.Where(x => x.UserId == userId && x.TenantId == tenantId)
				.ToListAsync(ct);
		}
		public async Task<List<DeviceToken>> GetByUserIdsAsync(IEnumerable<Guid> userIds, Guid tenantId, CancellationToken ct)
		{
			return await _db.DeviceTokens
				.Where(x => userIds.Contains(x.UserId) && x.TenantId == tenantId)
				.ToListAsync(ct);
		}
		public async Task RemoveByTokenAsync(string token, CancellationToken ct)
		{
			// Use IgnoreQueryFilters so we can find and remove the token regardless of query filters
			var existing = await _db.DeviceTokens
				.IgnoreQueryFilters()
				.FirstOrDefaultAsync(x => x.Token == token, ct);

			if (existing != null)
			{
				_db.DeviceTokens.Remove(existing);
			}
		}

	}
}
