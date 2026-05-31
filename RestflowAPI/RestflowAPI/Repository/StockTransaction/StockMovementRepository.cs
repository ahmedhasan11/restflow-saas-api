using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.DTOs.StockTransaction;
using RestflowAPI.Entities;
using RestflowAPI.Repository.Interfaces.StockTransaction;

namespace RestflowAPI.Repository.StockTransaction
{
    public class StockMovementRepository : IStockMovementRepository
    {
        private readonly ApplicationDbContext _db;
        public StockMovementRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(
    StockMovement entity,
    CancellationToken cancellationToken)
        {
            await _db.StockMovements
                .AddAsync(entity, cancellationToken);
        }

        public async Task<List<StockMovementDto>> GetByInventoryItemIdAsync(
     Guid itemId,
     Guid tenantId,
     CancellationToken cancellationToken)
        {
            return await _db.StockMovements
                .Where(x =>
                    x.InventoryItemId == itemId &&
                    x.TenantId == tenantId)
                .OrderByDescending(x => x.TransactionDate)
                .Select(x => new StockMovementDto
                {
                    Id = x.Id,
                    TransactionType = x.TransactionType,
                    Quantity = x.Quantity,
                    Note = x.Note,
                    TransactionDate = x.TransactionDate,
                    CreatedBy = x.Tenant.Users.FirstOrDefault(u => u.TenantId == x.CreatedBy)!.FullName ?? "System"
                })
                .ToListAsync(cancellationToken);
        }
    }
}
