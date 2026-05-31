using RestflowAPI.DTOs.Reports;

namespace RestflowAPI.ServiceInterfaces.Reports
{
	public interface IReportsService
	{
		Task<FinancialSummaryDto> GetFinancialSummaryAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);

		Task<List<ChartDataPointDto>> GetRevenueChartAsync(string period, CancellationToken cancellationToken);

		Task<List<MenuPerformanceDto>> GetMenuPerformanceAsync(DateTime fromDate, DateTime toDate, string sort, CancellationToken cancellationToken);

		Task<OperationalVolumeDto> GetOperationalVolumeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);
	}
}
