using RestflowAPI.Entities;

namespace RestflowAPI.Repository.Interfaces.Reports
{
	public interface IReportsRepository
	{
		Task<decimal> GetTotalRevenueAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);
		Task<List<Order>> GetCompletedOrdersInRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);
	}
}
