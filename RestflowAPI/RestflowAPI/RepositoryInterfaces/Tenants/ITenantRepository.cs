using RestflowAPI.Entities;

namespace RestflowAPI.RepositoryInterfaces.Tenants
{
	public interface ITenantRepository
	{
		Task CreateAsync(Tenant tenant, CancellationToken cancellationToken);
		Task<Tenant?> GetByCodeAsync(string code, CancellationToken cancellationToken);
	}
}
