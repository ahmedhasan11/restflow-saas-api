namespace RestflowAPI.DTOs.Reports
{
	public class MenuPerformanceDto
	{
		public Guid ProductId { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public decimal QuantitySold { get; set; }
	}
}
