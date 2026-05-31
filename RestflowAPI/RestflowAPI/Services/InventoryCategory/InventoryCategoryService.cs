using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.Inventory;
using RestflowAPI.DTOs.InventoryCategory;
using RestflowAPI.Entities;
using RestflowAPI.ServiceInterfaces;
using RestflowAPI.ServiceInterfaces.InventoryCategory;
using RestflowAPI.ServiceInterfaces.Tenants;
using RestflowAPI.Services.Tenants;

namespace RestflowAPI.Services;

public class InventoryCategoryService
    : IInventoryCategoryService
{
    private readonly IInventoryCategoryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentTenantService _tenantService;

    public InventoryCategoryService(
        IInventoryCategoryRepository repository, IUnitOfWork unitOfWork, ICurrentTenantService tenantService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _tenantService = tenantService;
    }

    public async Task<List<InventoryCategoryResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.TenantId
            ?? throw new Exception("Tenant is required");

        var categories = await _repository.GetAllAsync(cancellationToken);
        if (categories == null)
            throw new Exception("Product not found");
        return categories.Select(x => new InventoryCategoryResponseDto
        {
            Id = x.Id,
            CategoryName = x.CategoryName,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }).ToList();
    }

    public async Task<Guid> CreateAsync(CreateInventoryCategoryDto dto, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.TenantId
            ?? throw new Exception("Tenant is required");

        var category = new InventoryCategory
        {
            Id = Guid.NewGuid(),
            CategoryName = dto.CategoryName,
            CreatedAt = DateTime.UtcNow, 
        };

        await _repository.AddAsync(category);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return category.Id;
    }

    public async Task UpdateAsync(
        Guid id,
        UpdateInventoryCategoryDto dto, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.TenantId
            ?? throw new Exception("Tenant is required");

        var category = await _repository.GetByIdAsync(id, cancellationToken);

        if (category is null)
            throw new Exception("Category not found");

        category.CategoryName = dto.CategoryName;
        category.UpdatedAt = DateTime.UtcNow;

        _repository.Update(category);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}