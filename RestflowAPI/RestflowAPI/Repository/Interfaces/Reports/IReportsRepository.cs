using RestflowAPI.DTOs.Reports;
using RestflowAPI.Entities;

namespace RestflowAPI.Repository.Interfaces.Reports
{
	public interface IReportsRepository
	{
		Task<decimal> GetTotalRevenueAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);
		Task<List<Order>> GetCompletedOrdersInRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);
		Task<List<Product>> GetAllActiveProductsAsync(CancellationToken cancellationToken);
		Task<List<MenuPerformanceDto>> GetProductSalesVolumeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);
	}
}
