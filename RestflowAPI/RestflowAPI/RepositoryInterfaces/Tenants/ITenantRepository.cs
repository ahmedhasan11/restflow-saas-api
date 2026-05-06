using RestflowAPI.Entities;

namespace RestflowAPI.RepositoryInterfaces.Tenants
{
	public interface ITenantRepository
	{
		Task CreateAsync(Tenant tenant, CancellationToken cancellationToken);
		Task<Tenant?> GetByCodeAsync(string code, CancellationToken cancellationToken);
		Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
		Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken cancellationToken);
	}
}
