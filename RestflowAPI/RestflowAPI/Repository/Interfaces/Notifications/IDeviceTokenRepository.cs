using RestflowAPI.Entities;

namespace RestflowAPI.Repository.Interfaces.Notifications
{
	public interface IDeviceTokenRepository
	{
		Task UpsertAsync(DeviceToken token, CancellationToken ct);
		Task<List<DeviceToken>> GetByUserIdAsync(Guid userId, Guid tenantId, CancellationToken ct);
		Task<List<DeviceToken>> GetByUserIdsAsync(IEnumerable<Guid> userIds, Guid tenantId, CancellationToken ct);
		Task RemoveByTokenAsync(string token, CancellationToken ct);
	}
}
