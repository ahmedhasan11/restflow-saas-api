namespace RestflowAPI.DTOs.Reports
{
	public class OperationalVolumeDto
	{
		public StatusDistributionDto StatusDistribution { get; set; } = new();
		public List<OrderTypeMetricDto> OrderTypeMetrics { get; set; } = new();
	}
	public class StatusDistributionDto
	{
		public int Pending { get; set; }
		public int Completed { get; set; }
		public int Cancelled { get; set; }
	}
	public class OrderTypeMetricDto
	{
		public string OrderType { get; set; } = string.Empty;
		public int Count { get; set; }
		public decimal Revenue { get; set; }
		public decimal Percentage { get; set; }
	}
}
