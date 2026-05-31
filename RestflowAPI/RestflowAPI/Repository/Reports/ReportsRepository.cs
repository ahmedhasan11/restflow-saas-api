using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.DTOs.Reports;
using RestflowAPI.Entities;
using RestflowAPI.Enums;
using RestflowAPI.Repository.Interfaces.Reports;

namespace RestflowAPI.Repository.Reports
{
	public class ReportsRepository : IReportsRepository
	{
		private readonly ApplicationDbContext _db;
		public ReportsRepository(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<List<Product>> GetAllActiveProductsAsync(CancellationToken cancellationToken)
		{
			return await _db.Products.ToListAsync(cancellationToken);
		}

		public async Task<List<Order>> GetCompletedOrdersInRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
		{
			return await _db.Orders
				.Where(o => o.OrderStatus == OrderStatus.Completed
							&& o.CreatedAt >= fromDate
							&& o.CreatedAt < toDate)
				.OrderBy(o => o.CreatedAt)
				.ToListAsync(cancellationToken);
		}

		public async Task<List<MenuPerformanceDto>> GetProductSalesVolumeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
		{
			// Sum quantities from completed orders within range
			return await _db.OrderItems
				.Where(oi => oi.Order.OrderStatus == OrderStatus.Completed
							&& oi.Order.CreatedAt >= fromDate
							&& oi.Order.CreatedAt < toDate)
				.GroupBy(oi => oi.ProductId)
				.Select(g => new MenuPerformanceDto
				{
					ProductId = g.Key,
					// Use the maximum / latest ProductNameSnapshot from the items
					ProductName = g.Max(oi => oi.ProductNameSnapshot) ?? string.Empty,
					QuantitySold = g.Sum(oi => oi.Quantity ?? 0)
				})
				.ToListAsync(cancellationToken);
		}

		public async Task<decimal> GetTotalRevenueAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
		{

			var sum = await _db.Orders
				.Where(o => o.OrderStatus == OrderStatus.Completed
							&& o.CreatedAt >= fromDate
							&& o.CreatedAt < toDate)
				.SumAsync(o => o.TotalAmount ?? 0, cancellationToken);
			return sum;
		}

		public async Task<List<Order>> GetOrdersInRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
		{
			return await _db.Orders
				.Where(o => o.CreatedAt >= fromDate && o.CreatedAt < toDate)
				.ToListAsync(cancellationToken);
		}
	}
}
