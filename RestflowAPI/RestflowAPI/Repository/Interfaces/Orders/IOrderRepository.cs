using RestflowAPI.DTOs.Orders;
using RestflowAPI.Entities;
using RestflowAPI.Enums;

namespace RestflowAPI.Repository.Interfaces.Orders
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order, CancellationToken cancellationToken);

        Task<Order?> GetByIdAsync(
            Guid orderId,
            Guid tenantId,
            CancellationToken cancellationToken);

        Task<Order?> GetWithItemsAsync(
            Guid orderId,
            Guid tenantId,
            CancellationToken cancellationToken);

        Task<List<OrderListDto>> GetOrdersAsync(
            Guid tenantId,
            string? search,
            OrderStatus? status,
            PaymentStatus? paymentStatus,
            OrderType? orderType,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken);

        Task<string> GenerateOrderNumber();
       

        void Update(Order order);
    }
}
