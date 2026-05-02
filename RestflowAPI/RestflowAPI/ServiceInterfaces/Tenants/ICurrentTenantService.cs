namespace RestflowAPI.ServiceInterfaces.Tenants
{
	public interface ICurrentTenantService
	{
		Guid? TenantId { get; }
	}
}
