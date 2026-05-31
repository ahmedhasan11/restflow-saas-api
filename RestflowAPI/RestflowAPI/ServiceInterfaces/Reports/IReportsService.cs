using RestflowAPI.DTOs.Reports;

namespace RestflowAPI.ServiceInterfaces.Reports
{
	public interface IReportsService
	{
		Task<FinancialSummaryDto> GetFinancialSummaryAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);

		Task<List<ChartDataPointDto>> GetRevenueChartAsync(string period, CancellationToken cancellationToken);
	}
}
