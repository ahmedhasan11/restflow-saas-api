using RestflowAPI.DTOs.LowStockAlert;

namespace RestflowAPI.Repository.Interfaces.LowStockAlert
{
    public interface ILowStockAlertRepository
    {
        Task<List<LowStockAlertDto>> GetLowStockItemsAsync(
    Guid tenantId,
    CancellationToken cancellationToken);
    }
}
