using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.DTOs.InventoryItems;
using RestflowAPI.Repository.Interfaces.InventoryItem;
using System;

namespace RestflowAPI.Repository.InventoryItem
{

    public class InventoryItemRepository : IInventoryItemRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RestflowAPI.Entities.InventoryItem item, CancellationToken cancellationToken)
        {
            await _context.InventoryItems.AddAsync(item, cancellationToken);
        }

        public async Task<RestflowAPI.Entities.InventoryItem?> GetByIdAsync(Guid tenantId, Guid itemId, CancellationToken cancellationToken)
        {
            return await _context.InventoryItems
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x =>
                    x.Id == itemId &&
                    x.TenantId == tenantId &&
                    x.DeletedAt == null, cancellationToken);
        }

        public async Task<List<RestflowAPI.Entities.InventoryItem>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken)
        {
            return await _context.InventoryItems
                .Include(x => x.Category)
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.DeletedAt == null)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid tenantId, string itemName, CancellationToken cancellationToken)
        {
            return await _context.InventoryItems
                .AnyAsync(x =>
                    x.TenantId == tenantId &&
                    x.ItemName == itemName &&
                    x.DeletedAt == null, cancellationToken);
        }

        public void Update(RestflowAPI.Entities.InventoryItem item)
        {
            _context.InventoryItems.Update(item);
        }

        public async Task<List<InventoryItemListDto>>
        GetInventoryItemsAsync(
        Guid tenantId,
        string? search,
        Guid? categoryId,
        CancellationToken cancellationToken)
        {
            var query = _context.InventoryItems
                .Include(i => i.Category)
                .Where(i =>
                    i.TenantId == tenantId &&
                    i.DeletedAt == null);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(i =>
                    i.ItemName.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(i =>
                    i.CategoryId == categoryId.Value);
            }

            return await query
                .Select(i => new InventoryItemListDto
                {
                    Id = i.Id,
                    ItemName = i.ItemName,
                    CategoryName = i.Category.CategoryName,
                    UnitOfMeasure = i.UnitOfMeasure,
                    CurrentQuantity = i.CurrentQuantity,
                    MinimumQuantity = i.MinimumQuantity,
                    CostPerUnit = i.CostPerUnit,
                    IsLowStock =
                        i.CurrentQuantity <= i.MinimumQuantity
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<RestflowAPI.Entities.InventoryItem?> GetInventoryItemDetailsAsync(
       Guid id,
       Guid tenantId,
       CancellationToken cancellationToken)
        {
            return await _context.InventoryItems
                .Include(i => i.Category)
                .Include(i => i.StockMovements)
                .FirstOrDefaultAsync(i =>
                    i.Id == id &&
                    i.TenantId == tenantId &&
                    i.DeletedAt == null,
                    cancellationToken);
        }

        public void UpdateAsync(RestflowAPI.Entities.InventoryItem item)
        {
            _context.InventoryItems .Update(item);  
        }

        public async Task<List<InventoryItemResponseDto>> GetAllAsync(
    Guid tenantId,
    CancellationToken cancellationToken,
    string? search,
    Guid? categoryId)
        {
            var query = _context.InventoryItems
                .Include(x => x.Category)
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.DeletedAt == null)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.ItemName.Contains(search));
            }

            // Filter by category
            if (categoryId.HasValue)
            {
                query = query.Where(x =>
                    x.CategoryId == categoryId.Value);
            }

            return await query
                .OrderBy(x => x.ItemName)
                .Select(x => new InventoryItemResponseDto
                {
                    Id = x.Id,
                    ItemName = x.ItemName,
                    CategoryName = x.Category.CategoryName,
                    UnitOfMeasure = x.UnitOfMeasure,
                    CurrentQuantity = x.CurrentQuantity,
                    MinimumQuantity = x.MinimumQuantity,
                    CostPerUnit = x.CostPerUnit,
                    IsLowStock = x.CurrentQuantity <= x.MinimumQuantity
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> CategoryExistsAsync(
    Guid categoryId,
    CancellationToken cancellationToken)
        {
            return await _context.InventoryCategories
                .AnyAsync(x => x.Id == categoryId, cancellationToken);
        }

    }
}
