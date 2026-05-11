using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.Constants;
using RestflowAPI.DTOs.Product;

[Authorize(Policy =Permissions.Policies.TenantAccess)]
[ApiController]
[Route("menu/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductDto dto ,CancellationToken cancellationToken)
    {
        var id = await _productService.CreateProductAsync(dto,cancellationToken);
        return Ok(new { ProductId = id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, CreateProductDto dto, CancellationToken cancellationToken)
    {
        await _productService.UpdateProductAsync(id, dto, cancellationToken);
        return Ok(new { message = "Product updated successfully" });
    }
    [HttpPatch("{id}/availability")]
    public async Task<IActionResult> ChangeAvailability(Guid id, ChangeProductAvailabilityDto dto, CancellationToken cancellationToken)
    {
        await _productService.ChangeProductAvailabilityAsync(id, dto.IsAvailable, cancellationToken);
        return Ok(new { message = "Product availability updated" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(string? search, Guid? categoryId, CancellationToken cancellationToken)
    {
        var result = await _productService.GetProductsAsync(search, categoryId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetails(Guid id, CancellationToken cancellationToken)
    {
        var result = await _productService.GetProductDetailsAsync(id, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
