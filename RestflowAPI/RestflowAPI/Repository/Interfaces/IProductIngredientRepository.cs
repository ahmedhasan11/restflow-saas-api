using RestflowAPI.DTOs.Product;
using RestflowAPI.Entities;

public interface IProductIngredientRepository
{
    Task AddRangeAsync(IEnumerable<ProductIngredient> ingredients, CancellationToken cancellationToken);
    void RemoveRange(IEnumerable<ProductIngredient> ingredients);

    Task<Product?> GetProductAsync(Guid productId, Guid tenantId);

    Task<ProductIngredient?> GetIngredientAsync(Guid ingredientId, Guid productId, Guid tenantId);

    Task<bool> InventoryItemExistsAsync(Guid inventoryItemId, Guid tenantId);

    Task<int> CountByProductAsync(Guid productId, Guid tenantId);

    Task AddAsync(ProductIngredient entity);

    void Remove(ProductIngredient entity);

    Task<ProductIngredient?> GetByIdAsync(Guid id, Guid tenantId);
    Task<List<ProductIngredientDto>> GetProductIngredientsAsync(
    Guid productId,
    Guid tenantId,
    CancellationToken cancellationToken);
}