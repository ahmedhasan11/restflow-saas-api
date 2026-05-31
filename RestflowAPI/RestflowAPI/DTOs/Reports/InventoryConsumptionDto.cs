namespace RestflowAPI.DTOs.Reports
{
	public class InventoryConsumptionDto
	{
		public List<IngredientConsumptionDto> MostConsumedIngredients { get; set; } = new();
		public List<StockMovementSummaryDto> StockMovementSummaries { get; set; } = new();
	}
	public class IngredientConsumptionDto
	{
		public Guid InventoryItemId { get; set; }
		public string ItemName { get; set; } = string.Empty;
		public decimal TotalConsumption { get; set; }
		public string UnitOfMeasure { get; set; } = string.Empty;
	}
	public class StockMovementSummaryDto
	{
		public Guid InventoryItemId { get; set; }
		public string ItemName { get; set; } = string.Empty;
		public string UnitOfMeasure { get; set; } = string.Empty;
		public decimal TotalStockIn { get; set; }
		public decimal TotalStockOut { get; set; }
		public decimal TotalAdjustments { get; set; }
	}
}
