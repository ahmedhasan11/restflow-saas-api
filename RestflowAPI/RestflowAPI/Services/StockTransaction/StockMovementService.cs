using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.StockTransaction;
using RestflowAPI.Entities;
using RestflowAPI.Exceptions;
using RestflowAPI.Repository.Interfaces.InventoryItem;
using RestflowAPI.Repository.Interfaces.StockTransaction;
using RestflowAPI.ServiceInterfaces.Customers;
using RestflowAPI.ServiceInterfaces.Notifications;
using RestflowAPI.ServiceInterfaces.StockTransaction;
using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Services.StockTransaction
{
    public class StockMovementService : IStockMovementService
    {
        private readonly IInventoryItemRepository _inventoryRepo;
        private readonly IStockMovementRepository _movementRepo;
        private readonly ICurrentTenantService _tenant;
        private readonly IUnitOfWork _uow;
		private readonly INotificationsService _notificationService;
		private readonly ILogger<StockMovementService> _logger;

		public StockMovementService(
            IInventoryItemRepository inventoryRepo,
            IStockMovementRepository repo,
            ICurrentTenantService tenant,
            IUnitOfWork uow,
            INotificationsService notificationService,
            ILogger<StockMovementService> logger)
        {
            _inventoryRepo = inventoryRepo;
            _tenant = tenant;
            _uow = uow;
            _movementRepo = repo;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task CreateAsync(
    Guid inventoryItemId,
    CreateStockMovementDto dto,
    CancellationToken cancellationToken)
        {
            var tenantId = _tenant.TenantId
                ?? throw new UnauthorizedException("Tenant is required");

            var item = await _inventoryRepo.GetByIdAsync(
                inventoryItemId,
                tenantId,
                cancellationToken);

            if (item == null)
                throw new Exception("Inventory item not found");

            switch (dto.TransactionType)
            {
                case Enums.TransactionType.StockIn:

                    item.CurrentQuantity += dto.Quantity;
                    break;

                case Enums.TransactionType.StockOut:

                    if (item.CurrentQuantity < dto.Quantity)
                        throw new Exception("Insufficient stock");

                    item.CurrentQuantity -= dto.Quantity;
                    break;

                case Enums.TransactionType.Adjustment:

                    if (dto.Quantity < 0 &&
                        item.CurrentQuantity + dto.Quantity < 0)
                    {
                        throw new Exception("Negative stock is not allowed");
                    }

                    item.CurrentQuantity += dto.Quantity;
                    break;
            }

            var movement = new StockMovement
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                InventoryItemId = item.Id,
                TransactionType = dto.TransactionType,
                Quantity = dto.Quantity,
                Note = dto.Note,
                TransactionDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
            };

            await _movementRepo.AddAsync(
                movement,
                cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);


			// FR-01 & FR-02: Stock alerts after manual stock adjustment
			try
			{
				if (item.CurrentQuantity == 0)
				{
					await _notificationService.SendOutOfStockAlertAsync(item, tenantId, cancellationToken);
				}
				else if (item.CurrentQuantity <= item.MinimumQuantity)
				{
					await _notificationService.SendLowStockAlertAsync(item, tenantId, cancellationToken);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to send stock alerts for item {ItemId} after manual adjustment", item.Id);
			}
		}

        public async Task<List<StockMovementDto>> GetHistoryAsync(
    Guid inventoryItemId,
    CancellationToken cancellationToken)
        {
            var tenantId = _tenant.TenantId
                ?? throw new UnauthorizedException("Tenant is required");

            return await _movementRepo.GetByInventoryItemIdAsync(tenantId, inventoryItemId, cancellationToken);
        }
    }
}
