using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.LowStockAlert;
using RestflowAPI.Entities;
using RestflowAPI.Repository.Interfaces.InventoryItem;
using RestflowAPI.Repository.Interfaces.LowStockAlert;
using RestflowAPI.ServiceInterfaces.LowStockAlert;
using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Services.LowStockAlert
{
    public class LowStockAlertService : ILowStockAlertService
    {

        private readonly ILowStockAlertRepository _inventoryRepo;
        private readonly ICurrentTenantService _tenant;
        private readonly IUnitOfWork _uow;

        public LowStockAlertService(
            ILowStockAlertRepository repo,
            ICurrentTenantService tenant,
            IUnitOfWork uow)
        {
            _inventoryRepo = repo;
            _tenant = tenant;
            _uow = uow;
        }
        public async Task<List<LowStockAlertDto>> GetLowStockItemsAsync(
    CancellationToken cancellationToken)
        {
            var tenantId = _tenant.TenantId
                ?? throw new Exception("Tenant is required");

            return await _inventoryRepo.GetLowStockItemsAsync(
                tenantId,
                cancellationToken);
        }
    }
}
