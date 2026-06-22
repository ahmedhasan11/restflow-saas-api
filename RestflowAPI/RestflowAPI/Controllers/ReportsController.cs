using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.Constants;
using RestflowAPI.DTOs.Reports;
using RestflowAPI.ServiceInterfaces.Reports;

namespace RestflowAPI.Controllers
{
	[Authorize(Policy =Permissions.Policies.OwnerOnly)]
	[Route("api/[controller]")]
	[ApiController]
	public class ReportsController : ControllerBase
	{
		private readonly IReportsService _reportsService;
		public ReportsController(IReportsService reportsService)
		{
			_reportsService = reportsService;
		}

		[HttpGet("financial-summary")]
		public async Task<ActionResult<FinancialSummaryDto>> GetFinancialSummary([FromQuery] DateTime? from,[FromQuery] DateTime? to,CancellationToken cancellationToken)
		{
			if (!from.HasValue || !to.HasValue)
			{
				return BadRequest(new { message = "Both 'from' and 'to' query parameters are required." });
			}

			if (from.Value > to.Value)
			{
				return BadRequest(new { message = "The 'from' date must be less than or equal to the 'to' date." });
			}

			var summary = await _reportsService.GetFinancialSummaryAsync(from.Value, to.Value, cancellationToken);
			return Ok(summary);
		}

		[HttpGet("revenue-chart")]
		public async Task<ActionResult<List<ChartDataPointDto>>> GetRevenueChart([FromQuery] string period = "week", CancellationToken cancellationToken = default)
		{
			var supportedPeriods = new[] { "week", "month", "year" };
			if (string.IsNullOrWhiteSpace(period) || !supportedPeriods.Contains(period.ToLower()))
			{
				return BadRequest(new { message = "Invalid period. Supported values are 'week', 'month', or 'year'." });
			}
			try
			{
				var chartData = await _reportsService.GetRevenueChartAsync(period, cancellationToken);
				return Ok(chartData);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpGet("menu-performance")]
		public async Task<ActionResult<List<MenuPerformanceDto>>> GetMenuPerformance([FromQuery] DateTime? from,	[FromQuery] DateTime? to, [FromQuery] string sort = "desc",
															CancellationToken cancellationToken = default)
		{
			if (!from.HasValue || !to.HasValue)
			{
				return BadRequest(new { message = "Both 'from' and 'to' query parameters are required." });
			}
			if (from.Value > to.Value)
			{
				return BadRequest(new { message = "The 'from' date must be less than or equal to the 'to' date." });
			}
			var allowedSorts = new[] { "asc", "desc" };
			if (string.IsNullOrWhiteSpace(sort) || !allowedSorts.Contains(sort.ToLower()))
			{
				return BadRequest(new { message = "Invalid sort parameter. Supported values are 'asc' or 'desc'." });
			}
			var performance = await _reportsService.GetMenuPerformanceAsync(from.Value, to.Value, sort, cancellationToken);
			return Ok(performance);
		}
		[HttpGet("operational-volume")]
		public async Task<ActionResult<OperationalVolumeDto>> GetOperationalVolume([FromQuery] DateTime? from,[FromQuery] DateTime? to,CancellationToken cancellationToken = default)
		{
			if (!from.HasValue || !to.HasValue)
			{
				return BadRequest(new { message = "Both 'from' and 'to' query parameters are required." });
			}
			if (from.Value > to.Value)
			{
				return BadRequest(new { message = "The 'from' date must be less than or equal to the 'to' date." });
			}
			var volume = await _reportsService.GetOperationalVolumeAsync(from.Value, to.Value, cancellationToken);
			return Ok(volume);
		}


		[HttpGet("order-status-distribution")]
		public async Task<ActionResult<StatusDistributionDto>> GetOrderStatusDistribution([FromQuery] DateTime? from,	[FromQuery] DateTime? to,
		    CancellationToken cancellationToken = default)
		{
			if (!from.HasValue || !to.HasValue)
			{
				return BadRequest(new { message = "Both 'from' and 'to' query parameters are required." });
			}

			if (from.Value > to.Value)
			{
				return BadRequest(new { message = "The 'from' date must be less than or equal to the 'to' date." });
			}

			var distribution = await _reportsService.GetOrderStatusDistributionAsync(from.Value, to.Value, cancellationToken);
			return Ok(distribution);
		}

		[HttpGet("order-type-analysis")]
		public async Task<ActionResult<List<OrderTypeMetricDto>>> GetOrderTypeAnalysis([FromQuery] DateTime? from,[FromQuery] DateTime? to,
			CancellationToken cancellationToken = default)
		{
			if (!from.HasValue || !to.HasValue)
			{
				return BadRequest(new { message = "Both 'from' and 'to' query parameters are required." });
			}

			if (from.Value > to.Value)
			{
				return BadRequest(new { message = "The 'from' date must be less than or equal to the 'to' date." });
			}

			var analysis = await _reportsService.GetOrderTypeAnalysisAsync(from.Value, to.Value, cancellationToken);
			return Ok(analysis);
		}

		[HttpGet("inventory-consumption")]
		public async Task<ActionResult<InventoryConsumptionDto>> GetInventoryConsumption([FromQuery] DateTime? from,[FromQuery] DateTime? to,CancellationToken cancellationToken = default)
		{
			if (!from.HasValue || !to.HasValue)
			{
				return BadRequest(new { message = "Both 'from' and 'to' query parameters are required." });
			}
			if (from.Value > to.Value)
			{
				return BadRequest(new { message = "The 'from' date must be less than or equal to the 'to' date." });
			}
			var consumption = await _reportsService.GetInventoryConsumptionAsync(from.Value, to.Value, cancellationToken);
			return Ok(consumption);
		}
	}
}
