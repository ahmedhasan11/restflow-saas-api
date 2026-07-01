namespace RestflowAPI.DTOs.AI.Internal
{
    public class InventoryForecastData
    {
        public List<LowStockItem> Items { get; set; } = [];
    }

    public class LowStockItem
    {
        public string ItemName { get; set; } = "";

        public decimal CurrentQuantity { get; set; }

        public decimal MinimumQuantity { get; set; }

        public decimal DailyConsumption { get; set; }
    }
}