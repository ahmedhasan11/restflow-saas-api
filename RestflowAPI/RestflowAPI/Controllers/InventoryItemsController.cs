using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.DTOs.InventoryItems;
using RestflowAPI.ServiceInterfaces.InventoryItems;

namespace RestflowAPI.Controllers
{

    [ApiController]
    [Route("inventory/items")]
    //[Authorize(Roles = "Owner,Employee")]
    public class InventoryItemsController : ControllerBase
    {
        private readonly IInventoryItemService _service;

        public InventoryItemsController(IInventoryItemService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? search,
            Guid? categoryId,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetAllAsync(
                search,
                categoryId,
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetDetailsAsync(id, cancellationToken);

            return Ok(result);
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateInventoryItemDto dto,
            CancellationToken cancellationToken)
        {
            var id = await _service.CreateAsync(dto, cancellationToken);

            return Ok(id);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(
            Guid id,
            UpdateInventoryItemDto dto,
            CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(id, dto, cancellationToken);

            return Ok();
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _service.DeactivateAsync(id, cancellationToken);

            return Ok();
        }
    }
}
