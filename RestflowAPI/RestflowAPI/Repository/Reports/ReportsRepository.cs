using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
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

		public async Task<List<Order>> GetCompletedOrdersInRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
		{
			return await _db.Orders
				.Where(o => o.OrderStatus == OrderStatus.Completed
							&& o.CreatedAt >= fromDate
							&& o.CreatedAt < toDate)
				.OrderBy(o => o.CreatedAt)
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
	}
}
