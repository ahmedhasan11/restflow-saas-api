using RestflowAPI.Entities;

public interface ICategoryRepository
{
    Task<MenuCategory?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid id, Guid tenantId, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(Guid tenantId, string name, Guid? excludeId, CancellationToken cancellationToken);

    Task AddAsync(MenuCategory entity);

    void Update(MenuCategory entity);
    Task<List<MenuCategory>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken);

}