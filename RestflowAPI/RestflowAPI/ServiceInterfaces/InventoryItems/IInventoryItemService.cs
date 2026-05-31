using RestflowAPI.DTOs.InventoryItems;

namespace RestflowAPI.ServiceInterfaces.InventoryItems
{

    public interface IInventoryItemService
    {

        Task<Guid> CreateAsync(
    CreateInventoryItemDto dto,
    CancellationToken cancellationToken);

        Task UpdateAsync(
            Guid id,
            UpdateInventoryItemDto dto,
            CancellationToken cancellationToken);

        Task DeactivateAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task<List<InventoryItemResponseDto>> GetAllAsync(
            string? search,
            Guid? categoryId,
            CancellationToken cancellationToken);

        Task<InventoryItemResponseDto?> GetDetailsAsync(
            Guid id,
            CancellationToken cancellationToken);
    }
}
