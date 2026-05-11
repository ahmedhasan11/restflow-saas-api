using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.Constants;
using RestflowAPI.DTOs.MenuCategory;
using RestflowAPI.ServiceInterfaces.ImenuCategory;

namespace RestflowAPI.Controllers
{
	[Authorize(Policy = Permissions.Policies.TenantAccess)]
	[ApiController]
    [Route("api/menu/categories")]
    public class MenuCategoriesController : ControllerBase
    {
        private readonly IMenuCategoryService _service;

        public MenuCategoriesController(IMenuCategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _service.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(Guid id, CreateMenuCategoryDto dto, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(id, dto, cancellationToken);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateMenuCategoryDto dto, CancellationToken cancellationToken)
        {
            var menu = await _service.CreateAsync(dto, cancellationToken);
            return Ok(menu);
        }
    }

}