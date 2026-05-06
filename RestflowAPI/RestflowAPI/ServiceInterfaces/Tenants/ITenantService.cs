using RestflowAPI.DTOs.Tenants;

namespace RestflowAPI.ServiceInterfaces.Tenants
{
	public interface ITenantService
	{
		Task<TenantResponseDto> CreateTenantAsync(CreateTenantRequestDto request, CancellationToken cancellationToken);
	}
}
