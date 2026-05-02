using RestflowAPI.ServiceInterfaces.Tenants;
using System;
using System.Collections.Generic;

namespace RestflowAPI.Entities;

public class MenuCategory : BaseEntity, IMustHaveTenant
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }

    // Navigation Properties
    public Tenant Tenant { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
