using RestflowAPI.Entities;

namespace RestflowAPI.Repository.Interfaces.Settings
{
	public interface IPlatformRepository
	{
		Task<IEnumerable<PlatformSetting>> GetAllAsync(CancellationToken cancellationToken);
		Task<PlatformSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken);
		Task SaveAsync(PlatformSetting setting, CancellationToken cancellationToken);
	}
}
