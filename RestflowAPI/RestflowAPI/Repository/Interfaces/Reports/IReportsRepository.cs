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
		Task<List<Order>> GetOrdersInRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);

		Task<List<RestflowAPI.Entities.InventoryItem>> GetAllActiveInventoryItemsAsync(CancellationToken cancellationToken);
		Task<List<StockMovement>> GetStockMovementsInRangeAsync(DateTime fromDate, DateTime toDate, Guid tenantId, CancellationToken cancellationToken);
	}
}
