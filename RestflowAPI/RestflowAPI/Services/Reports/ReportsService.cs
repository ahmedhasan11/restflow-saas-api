using RestflowAPI.DTOs.Reports;
using RestflowAPI.Repository.Interfaces.Reports;
using RestflowAPI.ServiceInterfaces.Reports;

namespace RestflowAPI.Services.Reports
{
	public class ReportsService : IReportsService
	{
		private readonly IReportsRepository _reportsRepository;

		public ReportsService(IReportsRepository reportsRepository)
		{
			_reportsRepository = reportsRepository;
		}
		public async Task<FinancialSummaryDto> GetFinancialSummaryAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
		{
			var currentStart = fromDate.Date;
			var currentEnd = toDate.Date.AddDays(1);
			var duration = currentEnd - currentStart;

			var previousStart = currentStart - duration;
			var previousEnd = currentStart;

			var currentRevenue = await _reportsRepository.GetTotalRevenueAsync(currentStart, currentEnd, cancellationToken);
			var previousRevenue = await _reportsRepository.GetTotalRevenueAsync(previousStart, previousEnd, cancellationToken);

			string growthString;
			if (previousRevenue == 0)
			{
				growthString = currentRevenue == 0 ? "0.0%" : "+100.0%";
			}
			else
			{
				var growthPercentage = ((currentRevenue - previousRevenue) / previousRevenue) * 100;
				growthString = growthPercentage >= 0? $"+{growthPercentage:F1}%" : $"{growthPercentage:F1}%";
			}

			return new FinancialSummaryDto
			{
				TotalRevenue = currentRevenue,
				RevenueGrowth = growthString
			};
		}

		public async Task<List<ChartDataPointDto>> GetRevenueChartAsync(string period, CancellationToken cancellationToken)
		{
			DateTime fromDate;
			DateTime toDate = DateTime.UtcNow.Date.AddDays(1); // Exclude tomorrow
			List<ChartDataPointDto> buckets;

			switch (period.ToLower())
			{
				case "week":
					fromDate = DateTime.UtcNow.Date.AddDays(-6); // Last 7 days
					buckets = Enumerable.Range(0, 7)
						.Select(i => fromDate.AddDays(i))
						.Select(d => new ChartDataPointDto { Label = d.ToString("yyyy-MM-dd"), Amount = 0 })
						.ToList();
					break;
				case "month":
					fromDate = DateTime.UtcNow.Date.AddDays(-29); // Last 30 days
					buckets = Enumerable.Range(0, 30)
						.Select(i => fromDate.AddDays(i))
						.Select(d => new ChartDataPointDto { Label = d.ToString("yyyy-MM-dd"), Amount = 0 })
						.ToList();
					break;
				case "year":
					// Start of the month 11 months ago
					fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-11);
					buckets = Enumerable.Range(0, 12)
						.Select(i => fromDate.AddMonths(i))
						.Select(m => new ChartDataPointDto { Label = m.ToString("MMMM yyyy"), Amount = 0 })
						.ToList();
					break;
				default:
					throw new ArgumentException("Invalid period. Supported periods are 'week', 'month', or 'year'.");
			}

			var orders = await _reportsRepository.GetCompletedOrdersInRangeAsync(fromDate, toDate, cancellationToken);

			// Group and populate in-memory
			if (period.ToLower() == "year")
			{
				var grouped = orders
					.GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
					.ToDictionary(
						g => new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
						g => g.Sum(o => o.TotalAmount ?? 0)
					);
				foreach (var bucket in buckets)
				{
					if (grouped.TryGetValue(bucket.Label, out var sum))
					{
						bucket.Amount = sum;
					}
				}
			}
			else
			{
				// Daily grouping for week/month
				var grouped = orders
					.GroupBy(o => o.CreatedAt.Date)
					.ToDictionary(
						g => g.Key.ToString("yyyy-MM-dd"),
						g => g.Sum(o => o.TotalAmount ?? 0)
					);
				foreach (var bucket in buckets)
				{
					if (grouped.TryGetValue(bucket.Label, out var sum))
					{
						bucket.Amount = sum;
					}
				}
			}
			return buckets;
		}
	}
}
