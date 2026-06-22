using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.Constants;
using RestflowAPI.DTOs.Common;
using RestflowAPI.DTOs.Inventory;
using RestflowAPI.DTOs.InventoryCategory;
using RestflowAPI.ServiceInterfaces.InventoryCategory;

namespace RestflowAPI.Controllers
{
    [Route("api/inventory-categories")]
    [Authorize(Policy = Permissions.Policies.SuperAdminOnly)]
    [ApiController]
    public class InventoryCategoriesController : ControllerBase
    {
        private readonly IInventoryCategoryService _service;

        public InventoryCategoriesController(IInventoryCategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<InventoryCategoryResponseDto>>> GetAll(CancellationToken cancellationToken )
        {
            var result = await _service.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CreateInventoryCategoryDto dto, CancellationToken cancellationToken)
        {
            var id =  await _service.CreateAsync(dto, cancellationToken);
            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MessageResponse>> Update(Guid id, UpdateInventoryCategoryDto dto, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(id, dto, cancellationToken);
            return Ok(new MessageResponse { Message = "Inventory Category updated successfully" });
        }
    }
}
