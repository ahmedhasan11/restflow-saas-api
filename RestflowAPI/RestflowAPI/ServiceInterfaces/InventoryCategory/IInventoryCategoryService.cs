using RestflowAPI.DTOs.Inventory;
using RestflowAPI.DTOs.InventoryCategory;

namespace RestflowAPI.ServiceInterfaces.InventoryCategory
{
    public interface IInventoryCategoryService
    {
        Task<List<InventoryCategoryResponseDto>> GetAllAsync(CancellationToken cancellationToken);

        Task<Guid> CreateAsync(CreateInventoryCategoryDto dto, CancellationToken cancellationToken);

        Task UpdateAsync(Guid id, UpdateInventoryCategoryDto dto, CancellationToken cancellationToken);

    }
}
