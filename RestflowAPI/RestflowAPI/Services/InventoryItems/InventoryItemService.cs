using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.InventoryItems;
using RestflowAPI.Entities;
using RestflowAPI.Repository.Interfaces.InventoryItem;
using RestflowAPI.ServiceInterfaces.InventoryItems;
using RestflowAPI.ServiceInterfaces.Tenants;
using RestflowAPI.Services.Tenants;

namespace RestflowAPI.Services.InventoryItems
{

    public class InventoryItemService : IInventoryItemService
    {
        private readonly IInventoryItemRepository _repo;
        private readonly ICurrentTenantService _tenant;
        private readonly IInventoryCategoryRepository _categoryRepository;
        private readonly IUnitOfWork _uow;

        public InventoryItemService(
            IInventoryItemRepository repo,
            ICurrentTenantService tenant,
            IUnitOfWork uow,
            IInventoryCategoryRepository categoryRepository)
        {
            _repo = repo;
            _tenant = tenant;
            _uow = uow;
            _categoryRepository = categoryRepository;
        }

        public async Task<Guid> CreateAsync(
            CreateInventoryItemDto dto,
            CancellationToken cancellationToken)
        {
            var tenantId = _tenant.TenantId
                ?? throw new Exception("Tenant is required");

            var categoryExists = await _categoryRepository.GetByIdAsync(
                dto.CategoryId,
                cancellationToken);

            if (categoryExists == null)
                throw new Exception("Invalid category");

            var entity = new InventoryItem
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ItemName = dto.ItemName,
                CategoryId = dto.CategoryId,
                UnitOfMeasure = dto.UnitOfMeasure,
                CurrentQuantity = dto.CurrentQuantity,
                MinimumQuantity = dto.MinimumQuantity,
                CostPerUnit = dto.CostPerUnit,
            };

            await _repo.AddAsync(entity, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }

        public async Task UpdateAsync(
            Guid id,
            UpdateInventoryItemDto dto,
            CancellationToken cancellationToken)
        {
            var tenantId = _tenant.TenantId
                ?? throw new Exception("Tenant is required");

            var item = await _repo.GetByIdAsync(
                tenantId,
                id,
                cancellationToken);

            if (item == null)
                throw new Exception("Item not found");

            var categoryExists = await _categoryRepository.GetByIdAsync(
                dto.CategoryId,
                cancellationToken);

            if (categoryExists == null)
                throw new Exception("Invalid category");

            item.CategoryId = dto.CategoryId;
            item.ItemName = dto.ItemName;
            item.UnitOfMeasure = dto.UnitOfMeasure; 
            item.MinimumQuantity = dto.MinimumQuantity;
            item.CostPerUnit = dto.CostPerUnit;

             _repo.UpdateAsync(item);
            await _uow.SaveChangesAsync(cancellationToken);
        }

        public async Task DeactivateAsync(
    Guid id,
    CancellationToken cancellationToken)
        {
            var tenantId = _tenant.TenantId
                ?? throw new Exception("Tenant is required");

            var item = await _repo.GetByIdAsync(
                tenantId,
                id,
                cancellationToken);

            if (item == null)
                throw new Exception("Item not found");

            item.DeletedAt = DateTime.UtcNow;

             _repo.UpdateAsync(item);

            await _uow.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<InventoryItemResponseDto>> GetAllAsync(
            string? search,
            Guid? categoryId,
            CancellationToken cancellationToken)
        {
            var tenantId = _tenant.TenantId
                ?? throw new Exception("Tenant is required");

            return await _repo.GetAllAsync(
                tenantId,
                cancellationToken,
                search,
                categoryId);
        }

        public async Task<InventoryItemResponseDto?> GetDetailsAsync(
    Guid id,
    CancellationToken cancellationToken)
        {
            var tenantId = _tenant.TenantId
                ?? throw new Exception("Tenant is required");

            var item = await _repo.GetByIdAsync(
                tenantId,
                id,
                cancellationToken);

            if (item == null)
                return null;

            return new InventoryItemResponseDto
            {
                Id = item.Id,
                ItemName = item.ItemName,
                CategoryName = item.Category.CategoryName,
                UnitOfMeasure = item.UnitOfMeasure,
                CurrentQuantity = item.CurrentQuantity,
                MinimumQuantity = item.MinimumQuantity,
                CostPerUnit = item.CostPerUnit,
                IsActive = item.DeletedAt == null ? true: false,
            };
        }

        
    }
}
    