using RestflowAPI.ServiceInterfaces.Tenants;
using System;
using System.Collections.Generic;

namespace RestflowAPI.Entities;

public class Product : BaseEntity, IMustHaveTenant
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CategoryId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal SellingPrice { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;

    // Navigation Properties
    public Tenant Tenant { get; set; } = null!;
    public MenuCategory Category { get; set; } = null!;
    public ICollection<ProductIngredient> ProductIngredients { get; set; } = new List<ProductIngredient>();
}
