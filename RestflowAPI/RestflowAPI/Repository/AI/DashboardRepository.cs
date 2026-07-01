using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.DTOs.AI.Internal;
using RestflowAPI.Repository.Interfaces.AI;

namespace RestflowAPI.Repository.AI
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<InventoryForecastData> GetInventoryForecastAsync(
    CancellationToken cancellationToken)
        {
            var threeDaysAgo = DateTime.UtcNow.AddDays(-3);

            var items = await _context.InventoryItems
                .Select(item => new LowStockItem
                {
                    ItemName = item.ItemName,

                    CurrentQuantity = item.CurrentQuantity,

                    MinimumQuantity = item.MinimumQuantity,

                    DailyConsumption =
                        _context.StockMovements
                            .Where(x =>
                                x.InventoryItemId == item.Id &&
                                x.TransactionType == Enums.TransactionType.StockOut &&
                                x.TransactionDate >= threeDaysAgo)
                            .Sum(x => (decimal?)x.Quantity) / 3 ?? 0
                })
                .ToListAsync(cancellationToken);

            return new InventoryForecastData
            {
                Items = items
            };
        }

        public async Task<MenuEngineeringData> GetMenuEngineeringAsync(
    CancellationToken cancellationToken)
        {
            var sales = await _context.OrderItems
                .GroupBy(x => x.ProductNameSnapshot)
                .Select(g => new ProductSalesMetric
                {
                    ProductName = g.Key,

                    TotalOrders = g.Sum(x => (int)x.Quantity!)
                })
                .OrderByDescending(x => x.TotalOrders)
                .ToListAsync(cancellationToken);

            return new MenuEngineeringData
            {
                TopSelling = sales.Take(5).ToList(),

                LowestSelling = sales
                    .OrderBy(x => x.TotalOrders)
                    .Take(5)
                    .ToList()
            };
        }

        public async Task<PerformanceSummaryData> GetPerformanceSummaryAsync(
    CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            var todayOrders = await _context.Orders
                .Where(x => x.CreatedAt.Date == today)
                .ToListAsync(cancellationToken);

            var yesterdayRevenue = await _context.Orders
                .Where(x => x.CreatedAt.Date == yesterday)
                .SumAsync(x => x.TotalAmount ?? 0, cancellationToken);

            return new PerformanceSummaryData
            {
                TodayRevenue =
                    todayOrders.Sum(x => x.TotalAmount ?? 0),

                YesterdayRevenue =
                    yesterdayRevenue,

                TotalOrdersToday =
                    todayOrders.Count,

                DeliveryOrders =
                    todayOrders.Count(x =>
                        x.OrderType == Enums.OrderType.Delivery),

                DineInOrders =
                    todayOrders.Count(x =>
                        x.OrderType == Enums.OrderType.DineIn),

                TakeAwayOrders =
                    todayOrders.Count(x =>
                        x.OrderType == Enums.OrderType.Takeaway)
            };
        }
    }
}
