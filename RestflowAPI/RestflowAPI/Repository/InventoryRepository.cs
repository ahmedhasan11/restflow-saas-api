using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;

public class InventoryRepository : IInventoryRepository
{
    private readonly ApplicationDbContext _db;

    public InventoryRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<int> CountValidAsync(List<Guid> ids, Guid tenantId, CancellationToken cancellationToken)
    {
        return await _db.InventoryItems
            .CountAsync(i => ids.Contains(i.Id) && i.TenantId == tenantId, cancellationToken);
    }
}