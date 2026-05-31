using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.DTOs.Product;
using RestflowAPI.Entities;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _db;

    public ProductRepository(ApplicationDbContext db)
    {
        _db = db;
    }



    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _db.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product?> GetWithIngredientsAsync(Guid id, CancellationToken cancellationToken)
    {
        
        return await _db.Products
            .Include(p => p.ProductIngredients)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        

        await _db.Products.AddAsync(product, cancellationToken);
    }

    public void Update(Product product)
    {
        _db.Products.Update(product);
    }

    public async Task<List<ProductListDto>> GetProductsAsync(
    Guid tenantId,
    string? search,
    Guid? categoryId,
    CancellationToken cancellationToken)
    {
        var query = _db.Products
            .Include(p => p.Category)
            .Where(p => p.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.ProductName.Contains(search));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        return await query
            .Select(p => new ProductListDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                CategoryName = p.Category.CategoryName,
                SellingPrice = p.SellingPrice,
                IsAvailable = p.IsAvailable
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetProductDetailsAsync(Guid id, Guid? tenantId, CancellationToken cancellationToken)
    {
        return await _db.Products
            .Include(p => p.Category)
            .Include(p => p.ProductIngredients)
                .ThenInclude(pi => pi.InventoryItem)
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId, cancellationToken);
    }
}