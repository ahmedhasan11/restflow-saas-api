using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using RestflowAPI.Data;
using RestflowAPI.Entities;
using RestflowAPI.Repository.Interfaces.Settings;

namespace RestflowAPI.Repository.Settings
{
	public class PlatformRepository : IPlatformRepository
	{
		private ApplicationDbContext _db;
		public PlatformRepository(ApplicationDbContext db)
		{
			_db = db;
		}
		public async Task<IEnumerable<PlatformSetting>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _db.Set<PlatformSetting>().ToListAsync(cancellationToken);
		}

		public async Task<PlatformSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken)
		{
			return await _db.Set<PlatformSetting>()
				.FirstOrDefaultAsync(x => x.SettingKey == key, cancellationToken);
		}

		public async Task SaveAsync(PlatformSetting setting, CancellationToken cancellationToken)
		{
			var existing = await GetByKeyAsync(setting.SettingKey, cancellationToken);
			if (existing != null)
			{
				existing.SettingValue = setting.SettingValue;
				existing.IsSecret = setting.IsSecret;
				existing.UpdatedAt = DateTime.UtcNow;
			}
			else
			{
				await _db.Set<PlatformSetting>().AddAsync(setting, cancellationToken);
			}
		}
	}
}
