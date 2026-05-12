using System;
using RestflowAPI.Enums;
using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Entities;

public class Customer : BaseEntity, IMustHaveTenant
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

	public CustomerStatus Status { get; set; } = CustomerStatus.Active;

	// Navigation Properties
	public Tenant Tenant { get; set; } = null!;
}
