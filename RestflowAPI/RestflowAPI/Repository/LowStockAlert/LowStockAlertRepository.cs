using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.DTOs.LowStockAlert;
using RestflowAPI.Repository.Interfaces.LowStockAlert;

namespace RestflowAPI.Repository.LowStockAlert
{
    public class LowStockAlertRepository : ILowStockAlertRepository
    {
        private readonly ApplicationDbContext _db;
        public LowStockAlertRepository(ApplicationDbContext db) { _db = db; }
        public async Task<List<LowStockAlertDto>> GetLowStockItemsAsync(
    Guid tenantId,
    CancellationToken cancellationToken)
        {
            return await _db.InventoryItems
                .Include(x => x.Category)
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.DeletedAt == null &&
                    x.CurrentQuantity <= x.MinimumQuantity)
                .OrderBy(x => x.CurrentQuantity)
                .Select(x => new LowStockAlertDto
                {
                    Id = x.Id,
                    ItemName = x.ItemName,
                    CategoryName = x.Category.CategoryName,
                    CurrentQuantity = x.CurrentQuantity,
                    MinimumQuantity = x.MinimumQuantity,
                    UnitOfMeasure = x.UnitOfMeasure
                })
                .ToListAsync(cancellationToken);
        }
    }
}
