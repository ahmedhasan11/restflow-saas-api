using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.Product;
using RestflowAPI.Entities;
using RestflowAPI.ServiceInterfaces.ProductIngredient;
using RestflowAPI.ServiceInterfaces.Tenants;

public class ProductIngredientService : IProductIngredientService
{
    private readonly IProductIngredientRepository _repo;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _uow;

    public ProductIngredientService(
        IProductIngredientRepository repo,
        ICurrentTenantService tenant,
        IUnitOfWork uow)
    {
        _repo = repo;
        _tenant = tenant;
        _uow = uow;
    }

    public async Task AddAsync(AddProductIngredientDto dto, CancellationToken cancellationToken)
    {
        var tenantId = _tenant.TenantId
            ?? throw new Exception("Tenant is required");

        var product = await _repo.GetProductAsync(dto.ProductId, tenantId);

        if (product == null)
            throw new Exception("Product not found");

        var itemExists = await _repo.InventoryItemExistsAsync(
            dto.InventoryItemId,
            tenantId);

        if (!itemExists)
            throw new Exception("Inventory item not found");

        var entity = new ProductIngredient
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProductId = dto.ProductId,
            InventoryItemId = dto.InventoryItemId,
            QuantityRequired = dto.QuantityRequired
        };

        await _repo.AddAsync(entity);

        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Guid productId,
        Guid ingredientId,
        UpdateProductIngredientDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenant.TenantId
            ?? throw new Exception("Tenant is required");

        var ingredient = await _repo.GetByIdAsync(
            ingredientId,
            tenantId);

        if (ingredient == null)
            throw new Exception("Ingredient not found");

        if (dto.InventoryItemId.HasValue)
        {
            var exists = await _repo.InventoryItemExistsAsync(
                dto.InventoryItemId.Value,
                tenantId);

            if (!exists)
                throw new Exception("Inventory item not found");

            ingredient.InventoryItemId = dto.InventoryItemId.Value;
        }

        if (dto.QuantityRequired.HasValue)
        {
            ingredient.QuantityRequired = dto.QuantityRequired.Value;
        }

        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenant.TenantId ?? throw new Exception("Tenant is required");

        var ingredient = await _repo.GetByIdAsync(id, tenantId);
        if (ingredient == null)
            throw new Exception("Ingredient not found");

        var count = await _repo.CountByProductAsync(ingredient.ProductId, tenantId);

        if (count <= 1)
            throw new Exception("Product must have at least one ingredient");

        _repo.Remove(ingredient);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ProductIngredientDto>> GetByProductAsync(
    Guid productId,
    CancellationToken cancellationToken)
    {
        var tenantId = _tenant.TenantId
            ?? throw new Exception("Tenant is required");

        if (productId == Guid.Empty)
            throw new Exception("ProductId is required");

        var ingredients = await _repo.GetProductIngredientsAsync(
            productId,
            tenantId,
            cancellationToken);

        return ingredients;
    }
}