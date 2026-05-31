using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.DTOs.StockTransaction;
using RestflowAPI.ServiceInterfaces.StockTransaction;

namespace RestflowAPI.Controllers
{
    [ApiController]
    [Route("inventory/items")]
    //[Authorize(Roles = "Owner,Employee")]
    public class StockMovementsController : ControllerBase
    {
        private readonly IStockMovementService _service;

        public StockMovementsController(
            IStockMovementService service)
        {
            _service = service;
        }

        [HttpPost("{id}/transactions")]
        public async Task<IActionResult> Create(
            Guid id,
            CreateStockMovementDto dto,
            CancellationToken cancellationToken)
        {
            await _service.CreateAsync(
                id,
                dto,
                cancellationToken);

            return Ok();
        }

        [HttpGet("{id}/transactions")]
        public async Task<IActionResult> History(
            Guid id,
            CancellationToken cancellationToken)
        {
            return Ok(
                await _service.GetHistoryAsync(
                    id,
                    cancellationToken));
        }
    }
}
