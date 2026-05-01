using System;

namespace RestflowAPI.Entities;

public class Customer : BaseEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    // Navigation Properties
    public Tenant Tenant { get; set; } = null!;
}
