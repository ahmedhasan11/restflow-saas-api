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
	}
}
