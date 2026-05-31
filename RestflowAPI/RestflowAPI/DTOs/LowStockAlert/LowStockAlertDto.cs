namespace RestflowAPI.DTOs.LowStockAlert
{
    public class LowStockAlertDto
    {
        public Guid Id { get; set; }

        public string ItemName { get; set; } = null!;

        public string CategoryName { get; set; } = null!;

        public decimal CurrentQuantity { get; set; }

        public decimal MinimumQuantity { get; set; }

        public string UnitOfMeasure { get; set; } = null!;
    }
}
