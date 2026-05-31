using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.Constants;
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
		public async Task<IActionResult> GetFinancialSummary([FromQuery] DateTime? from,[FromQuery] DateTime? to,CancellationToken cancellationToken)
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
	}
}
