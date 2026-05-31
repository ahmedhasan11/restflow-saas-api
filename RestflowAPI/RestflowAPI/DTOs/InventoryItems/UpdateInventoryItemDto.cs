namespace RestflowAPI.DTOs.InventoryItems
{
    public class UpdateInventoryItemDto
    {
        public Guid CategoryId { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public string UnitOfMeasure { get; set; } = string.Empty;

        public decimal MinimumQuantity { get; set; }

        public decimal CostPerUnit { get; set; }

        //public byte[]? RowVersion { get; set; }
    }
}
