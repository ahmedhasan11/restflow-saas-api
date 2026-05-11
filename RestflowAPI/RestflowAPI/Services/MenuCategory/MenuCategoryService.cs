using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.MenuCategory;
using RestflowAPI.DTOs.MenuCategoryDtos;
using RestflowAPI.Entities;
using RestflowAPI.ServiceInterfaces.ImenuCategory;
using RestflowAPI.ServiceInterfaces.Tenants;

public class MenuCategoryService : IMenuCategoryService
{
    private readonly ICategoryRepository _repo;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _uow;

    public MenuCategoryService(
        ICategoryRepository repo,
        ICurrentTenantService tenant,
        IUnitOfWork uow)
    {
        _repo = repo;
        _tenant = tenant;
        _uow = uow;
    }

    public async Task<List<MenuCategoryDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var tenantId = _tenant.TenantId ?? throw new Exception("Tenant is required");

        var categories = await _repo.GetAllAsync(tenantId, cancellationToken);

        return categories.Select(c => new MenuCategoryDto
        {
            Id = c.Id,
            CategoryName = c.CategoryName,
            Description = c.Description,
            DisplayOrder = c.DisplayOrder
        }).ToList();
    }

    public async Task<Guid> CreateAsync(CreateMenuCategoryDto dto, CancellationToken cancellationToken)
    {
        var tenantId = _tenant.TenantId ?? throw new Exception("Tenant is required");

        if (string.IsNullOrWhiteSpace(dto.CategoryName))
            throw new Exception("Category name is required");

        var exists = await _repo.ExistsByNameAsync(tenantId, dto.CategoryName, null, cancellationToken);
        if (exists)
            throw new Exception("Category already exists");

        var category = new MenuCategory
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            CategoryName = dto.CategoryName,
            Description = dto.Description,
            DisplayOrder = dto.DisplayOrder ?? 0
        };

        await _repo.AddAsync(category);
        await _uow.SaveChangesAsync(cancellationToken);

        return category.Id;
    }

    public async Task UpdateAsync(Guid id, CreateMenuCategoryDto dto, CancellationToken cancellationToken)
    {
        var tenantId = _tenant.TenantId ?? throw new Exception("Tenant is required");

        var category = await _repo.GetByIdAsync(id, tenantId, cancellationToken);
        if (category == null)
            throw new Exception("Category not found");

        if (!string.IsNullOrWhiteSpace(dto.CategoryName))
        {
            var exists = await _repo.ExistsByNameAsync(tenantId, dto.CategoryName, id, cancellationToken);
            if (exists)
                throw new Exception("Category name already exists");

            category.CategoryName = dto.CategoryName;
        }

        if (dto.Description != null)
            category.Description = dto.Description;

        if (dto.DisplayOrder.HasValue)
            category.DisplayOrder = dto.DisplayOrder.Value;

        _repo.Update(category);

        await _uow.SaveChangesAsync(cancellationToken);
    }
}