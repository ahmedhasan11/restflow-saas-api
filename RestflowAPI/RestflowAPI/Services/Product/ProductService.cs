//using RestflowAPI.Data.UnitOfWork;
//using RestflowAPI.Data;
//using RestflowAPI.Entities;
//using RestflowAPI.ServiceInterfaces.Tenants;
//using Microsoft.EntityFrameworkCore;
//using RestflowAPI.Services.Tenants;
//using RestflowAPI.DTOs.Product;

//public class ProductService : IProductService
//{
//    private readonly ApplicationDbContext _db;
//    private readonly IUnitOfWork _unitOfWork;
//    private readonly ICurrentTenantService _tenantService;

//    public ProductService(ApplicationDbContext db,
//                          IUnitOfWork unitOfWork,
//                          ICurrentTenantService tenantService)
//    {
//        _db = db;
//        _unitOfWork = unitOfWork;
//        _tenantService = tenantService;
//    }

//    public async Task<Guid> CreateProductAsync(CreateProductDto dto, CancellationToken cancellationToken)
//    {
//        var tenantId = _tenantService.TenantId
//            ?? throw new Exception("Tenant is required");

//        // 🔴 1) Basic Validation
//        if (string.IsNullOrWhiteSpace(dto.ProductName))
//            throw new Exception("Product name is required");

//        if (dto.SellingPrice < 0)
//            throw new Exception("Price must be >= 0");

//        if (dto.Ingredients == null || !dto.Ingredients.Any())
//            throw new Exception("At least one ingredient is required");

//        // 🔴 2) Validate Category
//        var category = await _db.MenuCategories
//            .FirstOrDefaultAsync(c => c.Id == dto.CategoryId, cancellationToken);

//        if (category == null)
//            throw new Exception("Category not found");

//        if (category.TenantId != tenantId)
//            throw new Exception("Invalid category for this tenant");

//        // 🔴 3) Validate Ingredients
//        var inventoryIds = dto.Ingredients.Select(i => i.InventoryItemId).ToList();

//        var inventoryItems = await _db.InventoryItems
//            .Where(i => inventoryIds.Contains(i.Id))
//            .ToListAsync(cancellationToken);

//        if (inventoryItems.Count != inventoryIds.Count)
//            throw new Exception("Invalid inventory item");

//        if (inventoryItems.Any(i => i.TenantId != tenantId))
//            throw new Exception("Cross-tenant inventory not allowed");

//        foreach (var ing in dto.Ingredients)
//        {
//            if (ing.QuantityRequired <= 0)
//                throw new Exception("Quantity must be greater than 0");
//        }

//        // 🔴 4) Create Product
//        var product = new Product
//        {
//            Id = Guid.NewGuid(),
//            ProductName = dto.ProductName,
//            CategoryId = dto.CategoryId,
//            SellingPrice = dto.SellingPrice,
//            Description = dto.Description,
//            ImageUrl = dto.ImageUrl,
//            IsAvailable = dto.IsAvailable
//        };

//        await _db.Products.AddAsync(product, cancellationToken);

//        // 🔴 5) Create Ingredients
//        foreach (var ing in dto.Ingredients)
//        {
//            var pi = new ProductIngredient
//            {
//                Id = Guid.NewGuid(),
//                ProductId = product.Id,
//                InventoryItemId = ing.InventoryItemId,
//                QuantityRequired = ing.QuantityRequired,
//                UnitOfMeasure = "unit"
//            };

//            await _db.ProductIngredients.AddAsync(pi, cancellationToken);
//        }

//        // 🔴 6) Save
//        await _unitOfWork.SaveChangesAsync(cancellationToken);

//        return product.Id;
//    }
//    public async Task UpdateProductAsync(Guid productId, CreateProductDto dto, CancellationToken cancellationToken)
//    {
//        var tenantId = _tenantService.TenantId;

//        if (!tenantId.HasValue)
//            throw new Exception("Tenant is required");

//        // 1. Get product with ingredients
//        var product = await _db.Products
//            .Include(p => p.ProductIngredients)
//            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

//        if (product == null)
//            throw new Exception("Product not found");

//        // 2. Prevent cross-tenant access
//        if (product.TenantId != tenantId.Value)
//            throw new Exception("Cross-tenant update not allowed");

//        // 3. Validate category
//        var categoryExists = await _db.MenuCategories
//            .AnyAsync(c => c.Id == dto.CategoryId && c.TenantId == tenantId, cancellationToken);

//        if (!categoryExists)
//            throw new Exception("Invalid category");

//        // 4. Validate price
//        if (dto.SellingPrice < 0)
//            throw new Exception("Price must be >= 0");

//        // 5. Validate ingredients exist
//        if (dto.Ingredients == null || dto.Ingredients.Count == 0)
//            throw new Exception("At least one ingredient is required");

//        // 6. Validate inventory items
//        var inventoryIds = dto.Ingredients.Select(i => i.InventoryItemId).ToList();

//        var validInventoryCount = await _db.InventoryItems
//            .CountAsync(i => inventoryIds.Contains(i.Id) && i.TenantId == tenantId, cancellationToken);

//        if (validInventoryCount != inventoryIds.Count)
//            throw new Exception("Invalid inventory items detected");

//        // 7. Validate quantity > 0
//        if (dto.Ingredients.Any(i => i.QuantityRequired <= 0))
//            throw new Exception("Quantity must be greater than 0");

//        // 8. Update product fields
//        product.ProductName = dto.ProductName;
//        product.CategoryId = dto.CategoryId;
//        product.Description = dto.Description;
//        product.SellingPrice = dto.SellingPrice;
//        product.ImageUrl = dto.ImageUrl;
//        product.IsAvailable = dto.IsAvailable;

//        // 9. Remove old ingredients
//        _db.ProductIngredients.RemoveRange(product.ProductIngredients);

//        // 10. Add new ingredients
//        var newIngredients = dto.Ingredients.Select(i => new ProductIngredient
//        {
//            Id = Guid.NewGuid(),
//            TenantId = tenantId.Value,
//            ProductId = product.Id,
//            InventoryItemId = i.InventoryItemId,
//            QuantityRequired = i.QuantityRequired,
//            UnitOfMeasure = "unit"
//        });

//        await _db.ProductIngredients.AddRangeAsync(newIngredients, cancellationToken);

//        // 11. Save changes
//        await _unitOfWork.SaveChangesAsync(cancellationToken);
//    }

//    public async Task ChangeProductAvailabilityAsync(Guid productId, bool isAvailable, CancellationToken cancellationToken)
//    {
//        var tenantId = _tenantService.TenantId;

//        if (!tenantId.HasValue)
//            throw new Exception("Tenant is required");

//        // 1. Get product
//        var product = await _db.Products
//            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

//        if (product == null)
//            throw new Exception("Product not found");

//        // 2. Tenant isolation check
//        if (product.TenantId != tenantId.Value)
//            throw new Exception("Cross-tenant access denied");

//        // 3. Update availability only
//        product.IsAvailable = isAvailable;

//        await _unitOfWork.SaveChangesAsync(cancellationToken);
//    }

//    public async Task<List<ProductListDto>> GetProductsAsync(string? search, Guid? categoryId, CancellationToken cancellationToken)
//    {
//        var tenantId = _tenantService.TenantId;

//        var query = _db.Products
//            .Include(p => p.Category)
//            .Where(p => p.TenantId == tenantId);

//        if (!string.IsNullOrWhiteSpace(search))
//            query = query.Where(p => p.ProductName.Contains(search));

//        if (categoryId.HasValue)
//            query = query.Where(p => p.CategoryId == categoryId);

//        return await query
//            .Select(p => new ProductListDto
//            {
//                Id = p.Id,
//                ProductName = p.ProductName,
//                CategoryName = p.Category.CategoryName,
//                SellingPrice = p.SellingPrice,
//                IsAvailable = p.IsAvailable
//            })
//            .ToListAsync(cancellationToken);
//    }

//    public async Task<ProductDetailsDto?> GetProductDetailsAsync(Guid id, CancellationToken cancellationToken)
//    {
//        var tenantId = _tenantService.TenantId;

//        var product = await _db.Products
//            .Include(p => p.Category)
//            .Include(p => p.ProductIngredients)
//                .ThenInclude(pi => pi.InventoryItem)
//            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId, cancellationToken);

//        if (product == null)
//            return null;

//        return new ProductDetailsDto
//        {
//            Id = product.Id,
//            ProductName = product.ProductName,
//            Description = product.Description,
//            SellingPrice = product.SellingPrice,
//            ImageUrl = product.ImageUrl,
//            IsAvailable = product.IsAvailable,
//            CategoryName = product.Category.CategoryName,

//            Ingredients = product.ProductIngredients.Select(i => new ProductIngredientDto
//            {
//                InventoryItemName = i.InventoryItem.ItemName,
//                QuantityRequired = i.QuantityRequired
//            }).ToList()
//        };
//    }
//}

using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.Product;
using RestflowAPI.Entities;
using RestflowAPI.ServiceInterfaces.Tenants;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IInventoryRepository _inventoryRepo;
    private readonly IProductIngredientRepository _ingredientRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentTenantService _tenantService;

    public ProductService(
        IProductRepository productRepo,
        ICategoryRepository categoryRepo,
        IInventoryRepository inventoryRepo,
        IProductIngredientRepository ingredientRepo,
        IUnitOfWork unitOfWork,
        ICurrentTenantService tenantService)
    {
        _productRepo = productRepo;
        _categoryRepo = categoryRepo;
        _inventoryRepo = inventoryRepo;
        _ingredientRepo = ingredientRepo;
        _unitOfWork = unitOfWork;
        _tenantService = tenantService;
    }

    public async Task ChangeProductAvailabilityAsync(
    Guid productId,
    bool isAvailable,
    CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.TenantId
            ?? throw new Exception("Tenant is required");

        var product = await _productRepo.GetByIdAsync(productId, cancellationToken);

        if (product == null)
            throw new Exception("Product not found");

        if (product.TenantId != tenantId)
            throw new Exception("Cross-tenant access denied");

        product.IsAvailable = isAvailable;

        _productRepo.Update(product);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid> CreateProductAsync(
    CreateProductDto dto,
    CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.TenantId
            ?? throw new Exception("Tenant is required");

        var category = await _categoryRepo.GetByIdAsync(
            dto.CategoryId,
            tenantId,
            cancellationToken);

        if (category == null || category.TenantId != tenantId)
            throw new Exception("Invalid category");

        var inventoryIds = dto.Ingredients
            .Select(i => i.InventoryItemId)
            .ToList();

        var count = await _inventoryRepo.CountValidAsync(
            inventoryIds,
            tenantId,
            cancellationToken);

        if (count != inventoryIds.Count)
            throw new Exception("Invalid inventory items");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProductName = dto.ProductName,
            CategoryId = dto.CategoryId,
            SellingPrice = dto.SellingPrice,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            IsAvailable = dto.IsAvailable
        };

        await _productRepo.AddAsync(product, cancellationToken);

        var ingredients = dto.Ingredients.Select(i => new ProductIngredient
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProductId = product.Id,
            InventoryItemId = i.InventoryItemId,
            QuantityRequired = i.QuantityRequired
        });

        await _ingredientRepo.AddRangeAsync(ingredients, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }

    public async Task<ProductDetailsDto?> GetProductDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.TenantId;

        var product = await _productRepo.GetProductDetailsAsync(id, tenantId, cancellationToken);

        if (product == null)
            return null;

        return new ProductDetailsDto
        {
            Id = product.Id,
            ProductName = product.ProductName,
            Description = product.Description,
            SellingPrice = product.SellingPrice,
            ImageUrl = product.ImageUrl,
            IsAvailable = product.IsAvailable,
            CategoryName = product.Category.CategoryName,

            Ingredients = product.ProductIngredients.Select(i => new ProductIngredientDto
            {
                InventoryItemName = i.InventoryItem.ItemName,
                QuantityRequired = i.QuantityRequired
            }).ToList()
        };
    }

    public async Task<List<ProductListDto>> GetProductsAsync(
    string? search,
    Guid? categoryId,
    CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.TenantId
            ?? throw new Exception("Tenant is required");

        return await _productRepo.GetProductsAsync(
            tenantId,
            search,
            categoryId,
            cancellationToken);
    }

    public async Task UpdateProductAsync(Guid productId, CreateProductDto dto, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.TenantId
            ?? throw new Exception("Tenant is required");

        var product = await _productRepo.GetWithIngredientsAsync(productId, cancellationToken);

        if (product == null)
            throw new Exception("Product not found");

        if (product.TenantId != tenantId)
            throw new Exception("Cross-tenant update not allowed");

        var categoryValid = await _categoryRepo.ExistsAsync(dto.CategoryId, tenantId, cancellationToken);

        if (!categoryValid)
            throw new Exception("Invalid category");

        var inventoryIds = dto.Ingredients.Select(i => i.InventoryItemId).ToList();

        var count = await _inventoryRepo.CountValidAsync(inventoryIds, tenantId, cancellationToken);

        if (count != inventoryIds.Count)
            throw new Exception("Invalid inventory items");

        product.ProductName = dto.ProductName;
        product.CategoryId = dto.CategoryId;
        product.Description = dto.Description;
        product.SellingPrice = dto.SellingPrice;
        product.ImageUrl = dto.ImageUrl;
        product.IsAvailable = dto.IsAvailable;

        _ingredientRepo.RemoveRange(product.ProductIngredients);

        var newIngredients = dto.Ingredients.Select(i => new ProductIngredient
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProductId = product.Id,
            InventoryItemId = i.InventoryItemId,
            QuantityRequired = i.QuantityRequired
        });

        await _ingredientRepo.AddRangeAsync(newIngredients, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}