using System;
using System.Collections.Generic;
using RestflowAPI.Enums;

namespace RestflowAPI.Entities;

public class Tenant : BaseEntity
{
    public Guid Id { get; set; }
    public string TenantCode { get; set; } = string.Empty;
    public string RestaurantName { get; set; } = string.Empty;
    public TenantStatus Status { get; set; } = TenantStatus.Active;
    public string? RestaurantLogoUrl { get; set; }
    public string? CuisineType { get; set; }
    public string Country { get; set; } = string.Empty;
    public string DefaultLanguage { get; set; } = "en";
    public string Timezone { get; set; } = "UTC";
    public string Currency { get; set; } = "USD";

    // Navigation Properties
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public ICollection<MenuCategory> MenuCategories { get; set; } = new List<MenuCategory>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<ProductIngredient> ProductIngredients { get; set; } = new List<ProductIngredient>();
}
