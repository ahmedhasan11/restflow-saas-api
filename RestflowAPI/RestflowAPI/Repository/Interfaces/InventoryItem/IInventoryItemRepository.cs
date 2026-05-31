using RestflowAPI.DTOs.InventoryItems;
using RestflowAPI.Entities;

namespace RestflowAPI.Repository.Interfaces.InventoryItem
{
    public interface IInventoryItemRepository
    {

        Task AddAsync(RestflowAPI.Entities.InventoryItem item, CancellationToken cancellationToken);

        void UpdateAsync(RestflowAPI.Entities.InventoryItem item);

        Task<RestflowAPI.Entities.InventoryItem?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken);

        Task<List<InventoryItemResponseDto>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken, string? search, Guid? categoryid);

        Task<bool> CategoryExistsAsync(Guid categoryId,CancellationToken cancellationToken);

    }
}
