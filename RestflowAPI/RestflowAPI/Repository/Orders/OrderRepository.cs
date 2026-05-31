using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.DTOs.Orders;
using RestflowAPI.Entities;
using RestflowAPI.Enums;
using RestflowAPI.Repository.Interfaces.Orders;

namespace RestflowAPI.Repository.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(
            Order order,
            CancellationToken cancellationToken)
        {
            await _db.Orders.AddAsync(order, cancellationToken);
        }

        public async Task<Order?> GetByIdAsync(
            Guid orderId,
            Guid tenantId,
            CancellationToken cancellationToken)
        {
            return await _db.Orders
                .FirstOrDefaultAsync(x =>
                    x.Id == orderId &&
                    x.TenantId == tenantId,
                    cancellationToken);
        }

        public async Task<Order?> GetWithItemsAsync(
            Guid orderId,
            Guid tenantId,
            CancellationToken cancellationToken)
        {
            return await _db.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x =>
                    x.Id == orderId &&
                    x.TenantId == tenantId,
                    cancellationToken);
        }

        public void Update(Order order)
        {
            _db.Orders.Update(order);
        }

        public async Task<List<OrderListDto>> GetOrdersAsync(
            Guid tenantId,
            string? search,
            OrderStatus? status,
            PaymentStatus? paymentStatus,
            OrderType? orderType,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var query = _db.Orders
                .Where(x => x.TenantId == tenantId);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x =>
                    x.OrderNumber.Contains(search));

            if (status.HasValue)
                query = query.Where(x =>
                    x.OrderStatus == status.Value);

            if (paymentStatus.HasValue)
                query = query.Where(x =>
                    x.PaymentStatus == paymentStatus.Value);

            if (orderType.HasValue)
                query = query.Where(x =>
                    x.OrderType == orderType.Value);

            if (fromDate.HasValue)
                query = query.Where(x =>
                    x.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x =>
                    x.CreatedAt <= toDate.Value);

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new OrderListDto
                {
                    Id = x.Id,
                    OrderNumber = x.OrderNumber,
                    OrderType = x.OrderType,
                    OrderStatus = x.OrderStatus,
                    PaymentStatus = x.PaymentStatus,
                    TotalAmount = x.TotalAmount
                })
                .ToListAsync(cancellationToken);
        }

         public async Task<string> GenerateOrderNumber()
        {
            var lastOrder = await _db.Orders
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            var nextNumber = lastOrder == null
                ? 1
                : int.Parse(lastOrder.OrderNumber) + 1;

            return nextNumber.ToString("D8"); 
        }
    }
}
