using RestflowAPI.DTOs.MenuCategory;
using RestflowAPI.DTOs.MenuCategoryDtos;

namespace RestflowAPI.ServiceInterfaces.ImenuCategory
{
    public interface IMenuCategoryService
    {
        Task<List<MenuCategoryDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<Guid> CreateAsync(CreateMenuCategoryDto dto, CancellationToken cancellationToken);
        Task UpdateAsync(Guid id, CreateMenuCategoryDto dto, CancellationToken cancellationToken);
    }
}
