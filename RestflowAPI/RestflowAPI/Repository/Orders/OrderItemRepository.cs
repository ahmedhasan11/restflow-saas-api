using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.DTOs.Orders;
using RestflowAPI.Entities;
using RestflowAPI.Enums;
using RestflowAPI.Repository.Interfaces.Orders;

namespace RestflowAPI.Repository.Orders
{
    public class OrderItemRepository : IOrderItemsRepository
    {

        private readonly ApplicationDbContext _db;

        public OrderItemRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<OrderItem?> GetByIdAsync(Guid itemId, CancellationToken cancellationToken)
        {
            return await _db.OrderItems
                .FirstOrDefaultAsync(x => x.Id == itemId, cancellationToken);
        }

        public async Task AddAsync(OrderItem item, CancellationToken cancellationToken)
        {
            await _db.OrderItems.AddAsync(item, cancellationToken);
        }

        public void Remove(OrderItem item)
        {
            _db.OrderItems.Remove(item);
        }
    }
}
