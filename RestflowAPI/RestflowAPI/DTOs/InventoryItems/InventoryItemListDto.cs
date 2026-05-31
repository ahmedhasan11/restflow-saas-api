namespace RestflowAPI.DTOs.InventoryItems
{
    public class InventoryItemListDto
    {
        public Guid Id { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public string UnitOfMeasure { get; set; } = string.Empty;

        public decimal CurrentQuantity { get; set; }

        public decimal MinimumQuantity { get; set; }

        public decimal CostPerUnit { get; set; }

        public bool IsLowStock { get; set; }
    }
}
