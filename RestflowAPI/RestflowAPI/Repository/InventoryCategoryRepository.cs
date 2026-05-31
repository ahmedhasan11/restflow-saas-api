using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.Entities;

public class InventoryCategoryRepository : IInventoryCategoryRepository
{
    private readonly ApplicationDbContext _db;

    public InventoryCategoryRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<int> CountValidAsync(List<Guid> ids, Guid tenantId, CancellationToken cancellationToken)
    {
        return await _db.InventoryItems
            .CountAsync(i => ids.Contains(i.Id) && i.TenantId == tenantId, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        string categoryName,
        Guid? excludeId,
        CancellationToken cancellationToken)
    {
        return await _db.InventoryCategories
            .AnyAsync(x =>
                x.CategoryName == categoryName &&
                (!excludeId.HasValue || x.Id != excludeId),
                cancellationToken);
    }

    

    public async Task<List<InventoryCategory>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _db.InventoryCategories
            .OrderBy(x => x.CategoryName)
            .ToListAsync(cancellationToken);
    }

    public async Task<InventoryCategory?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _db.InventoryCategories
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    

    public async Task AddAsync(InventoryCategory category)
    {
        await _db.InventoryCategories.AddAsync(category);
    }

    public void Update(InventoryCategory category)
    {
        _db.InventoryCategories.Update(category);
    }
}