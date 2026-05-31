using RestflowAPI.DTOs.Reports;

namespace RestflowAPI.ServiceInterfaces.Reports
{
	public interface IReportsService
	{
		Task<FinancialSummaryDto> GetFinancialSummaryAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);
	}
}
