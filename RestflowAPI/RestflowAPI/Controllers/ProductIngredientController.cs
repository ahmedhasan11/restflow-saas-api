using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.DTOs.Product;
using RestflowAPI.ServiceInterfaces.ProductIngredient;

namespace RestflowAPI.Controllers
{
    [ApiController]
    [Route("api/product-ingredients")]
    public class ProductIngredientsController : ControllerBase
    {
        private readonly IProductIngredientService _service;

        public ProductIngredientsController(IProductIngredientService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddProductIngredientDto dto, CancellationToken cancellationToken)
        {
            await _service.AddAsync(dto, cancellationToken);
            return Ok();
        }

        [HttpPatch("menu/products/{productId}/ingredients/{ingredientId}")]
        public async Task<IActionResult> UpdateIngredient(Guid productId, Guid ingredientId,UpdateProductIngredientDto dto,
        CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(productId, ingredientId, dto, cancellationToken);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);
            return Ok();
        }

        [HttpGet("menu/products/{id}/ingredients")]
        public async Task<IActionResult> GetByProduct(
    Guid id,
    CancellationToken cancellationToken)
        {
            var result = await _service.GetByProductAsync(id, cancellationToken);

            return Ok(result);
        }
    }
}
