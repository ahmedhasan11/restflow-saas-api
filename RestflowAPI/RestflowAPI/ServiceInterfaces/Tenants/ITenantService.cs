using RestflowAPI.DTOs.Tenants;

namespace RestflowAPI.ServiceInterfaces.Tenants
{
	public interface ITenantService
	{
		Task<TenantResponseDto> CreateTenantAsync(CreateTenantRequestDto request, CancellationToken cancellationToken);

		Task<IEnumerable<TenantResponseDto>> GetAllTenantsAsync(CancellationToken cancellationToken);

		Task<TenantResponseDto> ChangeTenantStatusAsync(Guid tenantId, ChangeTenantStatusDto request, CancellationToken cancellationToken);
	}
}
