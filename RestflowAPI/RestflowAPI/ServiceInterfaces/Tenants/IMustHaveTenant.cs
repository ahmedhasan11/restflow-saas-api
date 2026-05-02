namespace RestflowAPI.ServiceInterfaces.Tenants
{
	public interface IMustHaveTenant
	{
		Guid TenantId { get; set; }
	}
}
