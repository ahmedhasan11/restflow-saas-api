namespace RestflowAPI.DTOs.Reports
{
	public class FinancialSummaryDto
	{
		public decimal TotalRevenue { get; set; }
		public string RevenueGrowth { get; set; } = "0.0%";
	}
}
