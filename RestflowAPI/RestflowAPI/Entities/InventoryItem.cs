using RestflowAPI.ServiceInterfaces.Tenants;
using System;
using System.Collections.Generic;

namespace RestflowAPI.Entities;

public class InventoryItem : BaseEntity, IMustHaveTenant
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CategoryId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal CurrentQuantity { get; set; }
    public decimal MinimumQuantity { get; set; }
    public decimal CostPerUnit { get; set; }
    public byte[]? RowVersion { get; set; }

    // Navigation Properties
    public Tenant Tenant { get; set; } = null!;
    public InventoryCategory Category { get; set; } = null!;
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public ICollection<ProductIngredient> ProductIngredients { get; set; } = new List<ProductIngredient>();
}
