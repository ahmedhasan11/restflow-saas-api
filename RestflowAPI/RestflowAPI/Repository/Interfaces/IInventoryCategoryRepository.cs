using RestflowAPI.Entities;

public interface IInventoryCategoryRepository
{
    Task<int> CountValidAsync(List<Guid> ids,Guid tenentid,  CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(
        string categoryName,
        Guid? excludeId,
        CancellationToken cancellationToken);

    Task<List<InventoryCategory>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<InventoryCategory?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
    Task AddAsync(InventoryCategory category);

    void Update(InventoryCategory category);

}