using System;
using System.Collections.Generic;

namespace RestflowAPI.Entities;

public class InventoryCategory : BaseEntity
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    // Navigation Properties
    public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
}
