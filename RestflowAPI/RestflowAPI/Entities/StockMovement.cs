using System;
using RestflowAPI.Enums;

namespace RestflowAPI.Entities;

public class StockMovement : BaseEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid InventoryItemId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public string? Note { get; set; }
    public string? ReferenceId { get; set; }
    public DateTime TransactionDate { get; set; }

    // Navigation Properties
    public Tenant Tenant { get; set; } = null!;
    public InventoryItem InventoryItem { get; set; } = null!;
}
