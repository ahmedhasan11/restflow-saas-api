using RestflowAPI.DTOs.StockTransaction;
using RestflowAPI.Entities;

namespace RestflowAPI.ServiceInterfaces.StockTransaction
{
    public interface IStockMovementService
    {
        Task CreateAsync(
        Guid inventoryItemId,
        CreateStockMovementDto dto,
        CancellationToken cancellationToken);

        Task<List<StockMovementDto>> GetHistoryAsync(
            Guid inventoryItemId,
            CancellationToken cancellationToken);
    }
}
