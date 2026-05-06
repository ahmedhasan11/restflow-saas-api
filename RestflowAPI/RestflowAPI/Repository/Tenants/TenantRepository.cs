using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.Entities;
using RestflowAPI.RepositoryInterfaces.Tenants;

namespace RestflowAPI.Repository.Tenants
{
	public class TenantRepository : ITenantRepository
	{
		private readonly ApplicationDbContext _db;

		public TenantRepository(ApplicationDbContext db)
		{
			_db = db;
		}
		public async Task CreateAsync(Tenant tenant, CancellationToken cancellationToken)
		{
			await _db.Tenants.AddAsync(tenant, cancellationToken);
		}

		public async Task<Tenant?> GetByCodeAsync(string code, CancellationToken cancellationToken)
		{
			return await _db.Tenants.FirstOrDefaultAsync(t => t.TenantCode == code, cancellationToken);
		}
	}
}
