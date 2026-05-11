using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.DTOs.Product;
using RestflowAPI.Entities;

public class ProductIngredientRepository : IProductIngredientRepository
{
    private readonly ApplicationDbContext _db;

    public ProductIngredientRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task AddRangeAsync(IEnumerable<ProductIngredient> ingredients, CancellationToken cancellationToken)
    {
        await _db.ProductIngredients.AddRangeAsync(ingredients, cancellationToken);
    }

    public void RemoveRange(IEnumerable<ProductIngredient> ingredients)
    {
        _db.ProductIngredients.RemoveRange(ingredients);
    }

    public async Task<Product?> GetProductAsync(Guid productId, Guid tenantId)
    {
        return await _db.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.TenantId == tenantId);
    }

    public async Task<ProductIngredient?> GetIngredientAsync(Guid ingredientId, Guid productId, Guid tenantId)
    {
        return await _db.ProductIngredients
            .FirstOrDefaultAsync(i =>
                i.Id == ingredientId &&
                i.ProductId == productId &&
                i.TenantId == tenantId);
    }

    public async Task<bool> InventoryItemExistsAsync(Guid inventoryItemId, Guid tenantId)
    {
        return await _db.InventoryItems
            .AnyAsync(i => i.Id == inventoryItemId && i.TenantId == tenantId);
    }

    public async Task<int> CountByProductAsync(Guid productId, Guid tenantId)
    {
        return await _db.ProductIngredients
            .CountAsync(x => x.ProductId == productId && x.TenantId == tenantId);
    }

    public async Task AddAsync(ProductIngredient entity)
    {
        await _db.ProductIngredients.AddAsync(entity);
    }

    public void Remove(ProductIngredient entity)
    {
        _db.ProductIngredients.Remove(entity);
    }
    public async Task<ProductIngredient?> GetByIdAsync(Guid id, Guid tenantId)
    {
        return await _db.ProductIngredients
            .FirstOrDefaultAsync(i => i.Id == id && i.TenantId == tenantId);
    }

    public async Task<List<ProductIngredientDto>> GetProductIngredientsAsync(
    Guid productId,
    Guid tenantId,
    CancellationToken cancellationToken)
    {
        return await _db.ProductIngredients
            .Where(pi =>
                pi.ProductId == productId &&
                pi.TenantId == tenantId)
            .Select(pi => new ProductIngredientDto
            {
                InventoryItemName = pi.InventoryItem.ItemName,
                QuantityRequired = pi.QuantityRequired
            })
            .ToListAsync(cancellationToken);
    }
}