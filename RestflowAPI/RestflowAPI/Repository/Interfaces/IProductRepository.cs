using RestflowAPI.DTOs.Product;
using RestflowAPI.Entities;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Product?> GetWithIngredientsAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Product product, CancellationToken cancellationToken);
    void Update(Product product);
    Task<List<ProductListDto>> GetProductsAsync(
    Guid tenantId,
    string? search,
    Guid? categoryId,
    CancellationToken cancellationToken);
    Task<Product?> GetProductDetailsAsync(Guid id, Guid? tenantId, CancellationToken cancellationToken);
}