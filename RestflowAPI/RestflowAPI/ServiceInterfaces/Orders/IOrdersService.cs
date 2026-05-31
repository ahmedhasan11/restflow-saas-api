using RestflowAPI.DTOs.Orders;
using RestflowAPI.Entities;
using RestflowAPI.Enums;

namespace RestflowAPI.ServiceInterfaces.Orders
{
    public interface IOrdersService
    {
        Task<Guid> CreateOrderAsync(
    CreateOrderDto dto,
    CancellationToken cancellationToken);

        Task<OrderListDto?> GetDetailsAsync(
     Guid orderId,
     CancellationToken cancellationToken);

        Task UpdateStatusAsync(
    Guid orderId,
    UpdateOrderStatusDto dto,
    CancellationToken cancellationToken);

       

       Task UpdatePaymentStatusAsync(
    Guid orderId,
    UpdatePaymentStatusDto dto,
    CancellationToken cancellationToken);

        Task<List<OrderListDto>> GetOrdersAsync(
        string? search,
        OrderStatus? status,
        PaymentStatus? paymentStatus,
        OrderType? orderType,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken);
    }
}
