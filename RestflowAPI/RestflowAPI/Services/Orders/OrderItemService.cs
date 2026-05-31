using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.Orders;
using RestflowAPI.Entities;
using RestflowAPI.Enums;
using RestflowAPI.Repository.Interfaces.Orders;
using RestflowAPI.Repository.Interfaces.StockTransaction;
using RestflowAPI.ServiceInterfaces.Orders;
using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Services.Orders
{
    public class OrderItemService : IOrderItemService
    {

        private readonly IOrderRepository _orderrepo;
        private readonly ICurrentTenantService _tenant;
        private readonly IProductRepository _productrepo;
        private readonly IStockMovementRepository _stockmovementrepo;
        private readonly IProductIngredientRepository _productingredientrepo;
        private readonly IOrderItemsRepository _orderItems;
        private readonly IUnitOfWork _uow;

        public OrderItemService(
            IOrderRepository orderrepo,
            ICurrentTenantService tenant,
            IOrderItemsRepository orderItems,
            IUnitOfWork uow,
            IProductRepository productrepo,
            IStockMovementRepository stockmovementrepo,
            IProductIngredientRepository productingredientrepo
            )
        {
            _orderrepo = orderrepo;
            _tenant = tenant;
            _uow = uow;
            _productrepo = productrepo;
            _stockmovementrepo = stockmovementrepo;
            _productingredientrepo = productingredientrepo;
            _orderItems = orderItems;
        }

        private static void RecalculateOrder(Order order)
        {
            order.Subtotal =
                order.OrderItems.Sum(x => x.LineTotal);

            order.TotalAmount =
                order.Subtotal;
        }
        public async Task AddItemAsync(
    Guid orderId,
    CreateOrderItemDto dto,
    CancellationToken cancellationToken)
        {
            var tenantId =
                _tenant.TenantId
                ?? throw new Exception("Tenant required");

            var order =
                await _orderrepo.GetWithItemsAsync(
                    orderId,
                    tenantId,
                    cancellationToken);

            if (order == null)
                throw new Exception("Order not found");

            if (order.OrderStatus != OrderStatus.Pending)
                throw new Exception("Order locked");

            var product =
                await _productrepo.GetByIdAsync(
                    dto.ProductId,
                    cancellationToken);

            if (product == null)
                throw new Exception("Product not found");

            if (!product.IsAvailable)
                throw new Exception("Product unavailable");

            if (product.TenantId != tenantId)
                throw new Exception("Cross tenant access");

            var existing =
                order.OrderItems.FirstOrDefault(x =>
                    x.ProductId == dto.ProductId);

            if (existing != null)
            {
                existing.Quantity += dto.Quantity;

                existing.LineTotal =
                    existing.Quantity *
                    existing.UnitPrice;
            }
            else
            {
                var item = new OrderItem
                {
                    Id = Guid.NewGuid(),

                    ProductId = product.Id,

                    ProductNameSnapshot =
                            product.ProductName,

                    UnitPrice =
                            product.SellingPrice,

                    Quantity =
                            dto.Quantity,

                    LineTotal =
                            dto.Quantity *
                            product.SellingPrice,
                    TenantId = tenantId,
                };
                await _orderItems.AddAsync(item, cancellationToken);
            }

            RecalculateOrder(order);

            await _uow.SaveChangesAsync(
                cancellationToken);
        }


        public async Task UpdateItemAsync(
    Guid orderId,
    Guid itemId,
    UpdateOrderItemDto dto,
    CancellationToken cancellationToken)
        {
            var tenantId =
                _tenant.TenantId
                ?? throw new Exception("Tenant required");

            var order =
                await _orderrepo.GetWithItemsAsync(
                    orderId,
                    tenantId,
                    cancellationToken);

            if (order == null)
                throw new Exception("Order not found");

            if (order.OrderStatus != OrderStatus.Pending)
                throw new Exception("Order is locked");

            if (dto.Quantity <= 0)
                throw new Exception("Quantity must be greater than zero");

            var item =
                order.OrderItems
                    .FirstOrDefault(x => x.Id == itemId);

            if (item == null)
                throw new Exception("Order item not found");

            item.Quantity = dto.Quantity;

            item.LineTotal =
                item.UnitPrice * item.Quantity;

            RecalculateOrder(order);
            await _uow.SaveChangesAsync(
                cancellationToken);
        }

        public async Task DeleteItemAsync(
    Guid orderId,
    Guid itemId,
    CancellationToken cancellationToken)
        {
            var tenantId =
                _tenant.TenantId
                ?? throw new Exception("Tenant required");

            var order =
                await _orderrepo.GetWithItemsAsync(
                    orderId,
                    tenantId,
                    cancellationToken);

            if (order == null)
                throw new Exception("Order not found");

            if (order.OrderStatus != OrderStatus.Pending)
                throw new Exception("Order is locked");

            var item =
                order.OrderItems
                    .FirstOrDefault(x => x.Id == itemId);

            if (item == null)
                throw new Exception("Order item not found");

            if (order.OrderItems.Count <= 1)
                throw new Exception(
                    "Order must contain at least one item");

            order.OrderItems.Remove(item);

            RecalculateOrder(order);

            await _uow.SaveChangesAsync(
                cancellationToken);
        }
    }
}
