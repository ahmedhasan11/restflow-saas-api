using RestflowAPI.Entities;

namespace RestflowAPI.Repository.Interfaces.Orders
{
    public interface IOrderItemsRepository
    {
        Task<OrderItem?> GetByIdAsync(Guid itemId, CancellationToken cancellationToken);
        Task AddAsync(OrderItem item, CancellationToken cancellationToken);
        void Remove(OrderItem item);
    }
}
