using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.ServiceInterfaces.InventoryCategory;
using RestflowAPI.ServiceInterfaces.LowStockAlert;

namespace RestflowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryAlertsController : ControllerBase
    {

        private readonly ILowStockAlertService _service;

        public InventoryAlertsController(ILowStockAlertService service)
        {
            _service = service;
        }
        [HttpGet("alerts/low-stock")]
        public async Task<IActionResult> GetLowStockItems(
    CancellationToken cancellationToken)
        {
            var result = await _service
                .GetLowStockItemsAsync(cancellationToken);

            return Ok(result);
        }
    }
}
