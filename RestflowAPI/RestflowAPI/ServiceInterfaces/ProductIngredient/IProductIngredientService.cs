using RestflowAPI.DTOs.Product;

namespace RestflowAPI.ServiceInterfaces.ProductIngredient
{
    public interface IProductIngredientService
    {
        Task AddAsync(AddProductIngredientDto dto, CancellationToken cancellationToken);

        Task UpdateAsync(Guid productId, Guid ingredientId, UpdateProductIngredientDto dto, CancellationToken cancellationToken);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<List<ProductIngredientDto>> GetByProductAsync(
    Guid productId,
    CancellationToken cancellationToken);
    }
}
