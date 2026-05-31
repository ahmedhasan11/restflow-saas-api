using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.Product;
using RestflowAPI.Entities;
using RestflowAPI.ServiceInterfaces.Tenants;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IInventoryCategoryRepository _inventoryRepo;
    private readonly IProductIngredientRepository _ingredientRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentTenantService _tenantService;

    public ProductService(
        IProductRepository productRepo,
        ICategoryRepository categoryRepo,
        IInventoryCategoryRepository inventoryRepo,
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

        if (category == null)
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