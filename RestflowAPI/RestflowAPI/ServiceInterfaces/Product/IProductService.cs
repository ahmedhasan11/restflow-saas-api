using RestflowAPI.DTOs.Product;

public interface IProductService
{
    Task<Guid> CreateProductAsync(CreateProductDto dto, CancellationToken cancellationToken);
    Task UpdateProductAsync(Guid productId, CreateProductDto dto, CancellationToken cancellationToken);
    Task ChangeProductAvailabilityAsync(Guid productId, bool isAvailable, CancellationToken cancellationToken);
    Task<List<ProductListDto>> GetProductsAsync(string? search, Guid? categoryId, CancellationToken cancellationToken);

    Task<ProductDetailsDto?> GetProductDetailsAsync(Guid id, CancellationToken cancellationToken);
}
