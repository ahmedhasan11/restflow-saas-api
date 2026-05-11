public interface IInventoryRepository
{
    Task<int> CountValidAsync(List<Guid> ids, Guid tenantId, CancellationToken cancellationToken);
}