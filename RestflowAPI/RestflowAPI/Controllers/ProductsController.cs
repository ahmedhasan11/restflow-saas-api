using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.Constants;
using RestflowAPI.DTOs.Common;
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
    public async Task<ActionResult<ProductCreationResponse>> CreateProduct(CreateProductDto dto ,CancellationToken cancellationToken)
    {
        var id = await _productService.CreateProductAsync(dto,cancellationToken);
        return Ok(new ProductCreationResponse { ProductId = id });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MessageResponse>> UpdateProduct(Guid id, CreateProductDto dto, CancellationToken cancellationToken)
    {
        await _productService.UpdateProductAsync(id, dto, cancellationToken);
        return Ok(new MessageResponse { Message = "Product updated successfully" });
    }
    [HttpPatch("{id}/availability")]
    public async Task<ActionResult<MessageResponse>> ChangeAvailability(Guid id, ChangeProductAvailabilityDto dto, CancellationToken cancellationToken)
    {
        await _productService.ChangeProductAvailabilityAsync(id, dto.IsAvailable, cancellationToken);
        return Ok(new MessageResponse { Message = "Product availability updated" });
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductListDto>>> GetAll(string? search, Guid? categoryId, CancellationToken cancellationToken)
    {
        var result = await _productService.GetProductsAsync(search, categoryId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDetailsDto>> GetDetails(Guid id, CancellationToken cancellationToken)
    {
        var result = await _productService.GetProductDetailsAsync(id, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
