using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.Entities;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _db;

    public CategoryRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<MenuCategory?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken)
    {
        return await _db.MenuCategories
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, Guid tenantId, CancellationToken cancellationToken)
    {
        return await _db.MenuCategories
            .AnyAsync(c => c.Id == id && c.TenantId == tenantId, cancellationToken);
    }
    public async Task<List<MenuCategory>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        return await _db.MenuCategories
            .Where(c => c.TenantId == tenantId)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(Guid tenantId, string name, Guid? excludeId, CancellationToken cancellationToken)
    {
        return await _db.MenuCategories.AnyAsync(c =>
            c.TenantId == tenantId &&
            c.CategoryName == name &&
            (excludeId == null || c.Id != excludeId),
            cancellationToken);
    }
    public async Task AddAsync(MenuCategory entity)
    {
        await _db.MenuCategories.AddAsync(entity);
    }

    public void Update(MenuCategory entity)
    {
        _db.MenuCategories.Update(entity);
    }
}