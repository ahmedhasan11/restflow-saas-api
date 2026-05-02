using RestflowAPI.ServiceInterfaces.Tenants;
using System;

namespace RestflowAPI.Entities;

public class ProductIngredient : BaseEntity, IMustHaveTenant
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
    public Guid InventoryItemId { get; set; }
    public decimal QuantityRequired { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;

    // Navigation Properties
    public Tenant Tenant { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public InventoryItem InventoryItem { get; set; } = null!;
}
