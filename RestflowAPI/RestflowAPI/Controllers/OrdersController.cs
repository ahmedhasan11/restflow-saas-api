using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.DTOs.Orders;
using RestflowAPI.Enums;
using RestflowAPI.ServiceInterfaces.Customers;
using RestflowAPI.ServiceInterfaces.Orders;

namespace RestflowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Owner,Employee")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _service;
        public OrdersController(IOrdersService service)
        {
            _service = service;
        }


        [HttpPost]
        public async Task<IActionResult> Create(
    CreateOrderDto dto,
    CancellationToken cancellationToken)
        {
            var id = await _service.CreateOrderAsync(
                dto,
                cancellationToken);

            return Ok(id);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
    string? search,
    OrderStatus? status,
    PaymentStatus? paymentStatus,
    OrderType? orderType,
    DateTime? fromDate,
    DateTime? toDate,
    CancellationToken cancellationToken)
        {
            var result =
                await _service.GetOrdersAsync(
                    search,
                    status,
                    paymentStatus,
                    orderType,
                    fromDate,
                    toDate,
                    cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
    Guid id,
    CancellationToken cancellationToken)
        {
            var result =
                await _service.GetDetailsAsync(
                    id,
                    cancellationToken);

            return Ok(result);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
    Guid id,
    UpdateOrderStatusDto dto,
    CancellationToken cancellationToken)
        {
            await _service.UpdateStatusAsync(
                id,
                dto,
                cancellationToken);

            return Ok();
        }

        [HttpPatch("{id}/payment-status")]
        public async Task<IActionResult> UpdatePaymentStatus(
    Guid id,
    UpdatePaymentStatusDto dto,
    CancellationToken cancellationToken)
        {
            await _service.UpdatePaymentStatusAsync(
                id,
                dto,
                cancellationToken);

            return Ok();
        }
    }


}
