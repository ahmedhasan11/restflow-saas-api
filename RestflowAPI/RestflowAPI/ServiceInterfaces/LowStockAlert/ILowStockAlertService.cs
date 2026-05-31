using RestflowAPI.DTOs.LowStockAlert;

namespace RestflowAPI.ServiceInterfaces.LowStockAlert
{
    public interface ILowStockAlertService
    {
        Task<List<LowStockAlertDto>> GetLowStockItemsAsync(
    CancellationToken cancellationToken);
    }
}
