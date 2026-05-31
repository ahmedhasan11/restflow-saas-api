using RestflowAPI.DTOs.Orders;

namespace RestflowAPI.ServiceInterfaces.Orders
{
    public interface IOrderItemService
    {
        Task AddItemAsync(
    Guid orderId,
    CreateOrderItemDto dto,
    CancellationToken cancellationToken);

        Task UpdateItemAsync(
    Guid orderId,
    Guid itemId,
    UpdateOrderItemDto dto,
    CancellationToken cancellationToken);

        Task DeleteItemAsync(
    Guid orderId,
    Guid itemId,
    CancellationToken cancellationToken);
    }
}
