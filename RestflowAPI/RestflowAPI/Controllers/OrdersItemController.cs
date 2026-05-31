using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.DTOs.Orders;
using RestflowAPI.ServiceInterfaces.Orders;

namespace RestflowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Owner,Employee")]
    public class OrdersItemController : ControllerBase
    {

        private readonly IOrderItemService _service;
        public OrdersItemController(IOrderItemService service)
        {
            _service = service;
        }

        [HttpPost("{id}/items")]
        public async Task<IActionResult> AddItem(
    Guid id,
    CreateOrderItemDto dto,
    CancellationToken cancellationToken)
        {
            await _service.AddItemAsync(
                id,
                dto,
                cancellationToken);

            return Ok();
        }

        [HttpPatch("{id}/items/{itemId}")]
        public async Task<IActionResult> UpdateItem(
    Guid id,
    Guid itemId,
    UpdateOrderItemDto dto,
    CancellationToken cancellationToken)
        {
            await _service.UpdateItemAsync(
                id,
                itemId,
                dto,
                cancellationToken);

            return Ok();
        }

        [HttpDelete("{id}/items/{itemId}")]
        public async Task<IActionResult> DeleteItem(
    Guid id,
    Guid itemId,
    CancellationToken cancellationToken)
        {
            await _service.DeleteItemAsync(
                id,
                itemId,
                cancellationToken);

            return Ok();
        }
    }
}
