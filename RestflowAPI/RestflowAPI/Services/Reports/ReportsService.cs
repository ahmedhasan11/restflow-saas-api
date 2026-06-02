using RestflowAPI.DTOs.Reports;
using RestflowAPI.Enums;
using RestflowAPI.Repository.Interfaces.Reports;
using RestflowAPI.ServiceInterfaces.Reports;
using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Services.Reports
{
	public class ReportsService : IReportsService
	{
		private readonly IReportsRepository _reportsRepository;
		private readonly ICurrentTenantService _tenantService;

		public ReportsService(IReportsRepository reportsRepository, ICurrentTenantService tenantService)
		{
			_reportsRepository = reportsRepository;
			_tenantService = tenantService;
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
				growthString = growthPercentage >= 0 ? $"+{growthPercentage:F1}%" : $"{growthPercentage:F1}%";
			}

			return new FinancialSummaryDto
			{
				TotalRevenue = currentRevenue,
				RevenueGrowth = growthString
			};
		}
		public async Task<List<ChartDataPointDto>> GetRevenueChartAsync(string period, CancellationToken cancellationToken)
		{
			DateTime fromDate;
			DateTime toDate = DateTime.UtcNow.Date.AddDays(1); // Exclude tomorrow
			List<ChartDataPointDto> buckets;

			switch (period.ToLower())
			{
				case "week":
					fromDate = DateTime.UtcNow.Date.AddDays(-6); // Last 7 days
					buckets = Enumerable.Range(0, 7)
						.Select(i => fromDate.AddDays(i))
						.Select(d => new ChartDataPointDto { Label = d.ToString("yyyy-MM-dd"), Amount = 0 })
						.ToList();
					break;
				case "month":
					fromDate = DateTime.UtcNow.Date.AddDays(-29); // Last 30 days
					buckets = Enumerable.Range(0, 30)
						.Select(i => fromDate.AddDays(i))
						.Select(d => new ChartDataPointDto { Label = d.ToString("yyyy-MM-dd"), Amount = 0 })
						.ToList();
					break;
				case "year":
					// Start of the month 11 months ago
					fromDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-11);
					buckets = Enumerable.Range(0, 12)
						.Select(i => fromDate.AddMonths(i))
						.Select(m => new ChartDataPointDto { Label = m.ToString("MMMM yyyy"), Amount = 0 })
						.ToList();
					break;
				default:
					throw new ArgumentException("Invalid period. Supported periods are 'week', 'month', or 'year'.");
			}

			var orders = await _reportsRepository.GetCompletedOrdersInRangeAsync(fromDate, toDate, cancellationToken);

			// Group and populate in-memory
			if (period.ToLower() == "year")
			{
				var grouped = orders
					.GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
					.ToDictionary(
						g => new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
						g => g.Sum(o => o.TotalAmount ?? 0)
					);
				foreach (var bucket in buckets)
				{
					if (grouped.TryGetValue(bucket.Label, out var sum))
					{
						bucket.Amount = sum;
					}
				}
			}
			else
			{
				// Daily grouping for week/month
				var grouped = orders
					.GroupBy(o => o.CreatedAt.Date)
					.ToDictionary(
						g => g.Key.ToString("yyyy-MM-dd"),
						g => g.Sum(o => o.TotalAmount ?? 0)
					);
				foreach (var bucket in buckets)
				{
					if (grouped.TryGetValue(bucket.Label, out var sum))
					{
						bucket.Amount = sum;
					}
				}
			}
			return buckets;
		}
		public async Task<List<MenuPerformanceDto>> GetMenuPerformanceAsync(DateTime fromDate, DateTime toDate, string sort, CancellationToken cancellationToken)
		{
			var currentStart = fromDate.Date;
			var currentEnd = toDate.Date.AddDays(1);

			var activeProducts = await _reportsRepository.GetAllActiveProductsAsync(cancellationToken);

			var salesVolume = await _reportsRepository.GetProductSalesVolumeAsync(currentStart, currentEnd, cancellationToken);

			var performanceMap = activeProducts.ToDictionary(
				p => p.Id,
				p => new MenuPerformanceDto
				{
					ProductId = p.Id,
					ProductName = p.ProductName,
					QuantitySold = 0
				}
			);

			foreach (var sale in salesVolume)
			{
				if (performanceMap.TryGetValue(sale.ProductId, out var dto))
				{
					dto.QuantitySold = sale.QuantitySold;
				}
				else
				{
					// Historical product that is currently hidden/deleted but has sales in the period
					performanceMap[sale.ProductId] = new MenuPerformanceDto
					{
						ProductId = sale.ProductId,
						ProductName = sale.ProductName,
						QuantitySold = sale.QuantitySold
					};
				}
			}
			var resultList = performanceMap.Values.ToList();

			// 4. Sort based on parameter
			if (sort.ToLower() == "asc")
			{
				return resultList.OrderBy(p => p.QuantitySold).ThenBy(p => p.ProductName).ToList();
			}
			else
			{
				return resultList.OrderByDescending(p => p.QuantitySold).ThenBy(p => p.ProductName).ToList();
			}
		}
		public async Task<OperationalVolumeDto> GetOperationalVolumeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
		{
			var start = fromDate.Date;
			var end = toDate.Date.AddDays(1);
			var orders = await _reportsRepository.GetOrdersInRangeAsync(start, end, cancellationToken);

			var statusDistribution = new StatusDistributionDto
			{
				Pending = orders.Count(o => o.OrderStatus == OrderStatus.Pending),
				Completed = orders.Count(o => o.OrderStatus == OrderStatus.Completed),
				Cancelled = orders.Count(o => o.OrderStatus == OrderStatus.Cancelled)
			};

			var completedOrders = orders.Where(o => o.OrderStatus == OrderStatus.Completed).ToList();
			var totalCompletedRevenue = completedOrders.Sum(o => o.TotalAmount ?? 0);

			var orderTypes = new[] { OrderType.DineIn, OrderType.Takeaway, OrderType.Delivery };
			var orderTypeMetrics = new List<OrderTypeMetricDto>();

			foreach (var type in orderTypes)
			{
				var typeOrders = completedOrders.Where(o => o.OrderType == type).ToList();
				var typeRevenue = typeOrders.Sum(o => o.TotalAmount ?? 0);
				var typeCount = typeOrders.Count;
				decimal percentage = 0;
				if (totalCompletedRevenue > 0)
				{
					percentage = Math.Round((typeRevenue / totalCompletedRevenue) * 100, 2);
				}
				orderTypeMetrics.Add(new OrderTypeMetricDto
				{
					OrderType = type.ToString(),
					Count = typeCount,
					Revenue = typeRevenue,
					Percentage = percentage
				});
			}
			return new OperationalVolumeDto
			{
				StatusDistribution = statusDistribution,
				OrderTypeMetrics = orderTypeMetrics
			};
		}
		public async Task<InventoryConsumptionDto> GetInventoryConsumptionAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
		{
			var tenantId = _tenantService.TenantId ?? throw new Exception("Tenant context is required.");

			var start = fromDate.Date;
			var end = toDate.Date.AddDays(1);

			var activeItems = await _reportsRepository.GetAllActiveInventoryItemsAsync(cancellationToken);

			var movements = await _reportsRepository.GetStockMovementsInRangeAsync(start, end, tenantId, cancellationToken);

			var summaryMap = activeItems.ToDictionary(
				i => i.Id,
				i => new StockMovementSummaryDto
				{
					InventoryItemId = i.Id,
					ItemName = i.ItemName,
					UnitOfMeasure = i.UnitOfMeasure,
					TotalStockIn = 0,
					TotalStockOut = 0,
					TotalAdjustments = 0
				}
			);

			var consumptionMap = new Dictionary<Guid, decimal>();

			foreach (var movement in movements)
			{
				// Skip if there is no linked inventory item data
				if (movement.InventoryItem == null) continue;
				var itemId = movement.InventoryItemId;
				// If it's a soft-deleted item, it won't be in summaryMap, so add it dynamically
				if (!summaryMap.TryGetValue(itemId, out var summaryDto))
				{
					summaryDto = new StockMovementSummaryDto
					{
						InventoryItemId = itemId,
						ItemName = movement.InventoryItem.ItemName,
						UnitOfMeasure = movement.InventoryItem.UnitOfMeasure,
						TotalStockIn = 0,
						TotalStockOut = 0,
						TotalAdjustments = 0
					};
					summaryMap[itemId] = summaryDto;
				}
				// Aggregate by type
				if (movement.TransactionType == TransactionType.StockIn)
				{
					summaryDto.TotalStockIn += movement.Quantity;
				}
				else if (movement.TransactionType == TransactionType.StockOut)
				{
					summaryDto.TotalStockOut += movement.Quantity;

					// Add to consumption map
					if (!consumptionMap.ContainsKey(itemId)) consumptionMap[itemId] = 0;
					consumptionMap[itemId] += movement.Quantity;
				}
				else if (movement.TransactionType == TransactionType.Adjustment)
				{
					summaryDto.TotalAdjustments += movement.Quantity;
					// Add negative adjustments to consumption map
					if (movement.Quantity < 0)
					{
						if (!consumptionMap.ContainsKey(itemId)) consumptionMap[itemId] = 0;
						consumptionMap[itemId] += -movement.Quantity; // convert to positive consumption volume
					}
				}
			}
			// 4. Build Ranked Most Consumed Ingredients List
			var consumedIngredients = new List<IngredientConsumptionDto>();
			foreach (var kvp in consumptionMap)
			{
				if (kvp.Value > 0 && summaryMap.TryGetValue(kvp.Key, out var summary))
				{
					consumedIngredients.Add(new IngredientConsumptionDto
					{
						InventoryItemId = kvp.Key,
						ItemName = summary.ItemName,
						TotalConsumption = kvp.Value,
						UnitOfMeasure = summary.UnitOfMeasure
					});
				}
			}
			var rankedConsumption = consumedIngredients.OrderByDescending(c => c.TotalConsumption).ThenBy(c => c.ItemName).ToList();
			return new InventoryConsumptionDto
			{
				MostConsumedIngredients = rankedConsumption,
				StockMovementSummaries = summaryMap.Values.ToList()
			};
		}

		public async Task<StatusDistributionDto> GetOrderStatusDistributionAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
		{
			var start = fromDate.Date;
			var end = toDate.Date.AddDays(1);

			var orders = await _reportsRepository.GetOrdersInRangeAsync(start, end, cancellationToken);

			var statusDistribution = new StatusDistributionDto
			{
				Pending = orders.Count(o => o.OrderStatus == OrderStatus.Pending),
				Completed = orders.Count(o => o.OrderStatus == OrderStatus.Completed),
				Cancelled = orders.Count(o => o.OrderStatus == OrderStatus.Cancelled)
			};

			return statusDistribution;
		}

		public async Task<List<OrderTypeMetricDto>> GetOrderTypeAnalysisAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
		{
			var start = fromDate.Date;
			var end = toDate.Date.AddDays(1);

			var orders = await _reportsRepository.GetOrdersInRangeAsync(start, end, cancellationToken);

			var completedOrders = orders.Where(o => o.OrderStatus == OrderStatus.Completed).ToList();
			var totalCompletedRevenue = completedOrders.Sum(o => o.TotalAmount ?? 0);

			var orderTypes = new[] { OrderType.DineIn, OrderType.Takeaway, OrderType.Delivery };
			var orderTypeMetrics = new List<OrderTypeMetricDto>();

			foreach (var type in orderTypes)
			{
				var typeOrders = completedOrders.Where(o => o.OrderType == type).ToList();
				var typeRevenue = typeOrders.Sum(o => o.TotalAmount ?? 0);
				var typeCount = typeOrders.Count;

				decimal percentage = 0;
				if (totalCompletedRevenue > 0)
				{
					percentage = Math.Round((typeRevenue / totalCompletedRevenue) * 100, 2);
				}

				orderTypeMetrics.Add(new OrderTypeMetricDto
				{
					OrderType = type.ToString(),
					Count = typeCount,
					Revenue = typeRevenue,
					Percentage = percentage
				});
			}

			return orderTypeMetrics;
		}
	}
}