namespace RestflowAPI.DTOs.InventoryItems
{
    public class CreateInventoryItemDto
    {
        public string ItemName { get; set; } 

        public Guid CategoryId { get; set; }

        public string UnitOfMeasure { get; set; } 

        public decimal CurrentQuantity { get; set; }

        public decimal MinimumQuantity { get; set; }

        public decimal CostPerUnit { get; set; }
    }
}
