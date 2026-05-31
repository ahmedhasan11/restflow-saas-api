using RestflowAPI.DTOs.StockTransaction;
using RestflowAPI.Entities;

namespace RestflowAPI.Repository.Interfaces.StockTransaction
{
    public interface IStockMovementRepository
    {
        Task AddAsync(StockMovement transaction, CancellationToken cancellationToken);

        Task<List<StockMovementDto>> GetByInventoryItemIdAsync(Guid itemId, Guid tenantId, CancellationToken cancellationToken);
    }
}
