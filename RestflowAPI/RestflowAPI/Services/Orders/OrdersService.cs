using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data.UnitOfWork;
using RestflowAPI.DTOs.Orders;
using RestflowAPI.Entities;
using RestflowAPI.Enums;
using RestflowAPI.Exceptions;
using RestflowAPI.Repository.Interfaces.Orders;
using RestflowAPI.Repository.Interfaces.StockTransaction;
using RestflowAPI.Repository.Orders;
using RestflowAPI.ServiceInterfaces.Notifications;
using RestflowAPI.ServiceInterfaces.Orders;
using RestflowAPI.ServiceInterfaces.Tenants;
using RestflowAPI.Services.Notifications;

namespace RestflowAPI.Services.Orders
{
    public class OrdersService : IOrdersService
    {
        private readonly IOrderRepository _orderrepo;
        private readonly ICurrentTenantService _tenant;
        private readonly IProductRepository _productrepo;
        private readonly IStockMovementRepository _stockmovementrepo;
        private readonly IProductIngredientRepository _productingredientrepo;
        private readonly ILogger<OrdersService> _logger;
        private readonly IUnitOfWork _uow;
        private readonly INotificationsService _notificationsService;

        public OrdersService(
            IOrderRepository orderrepo,
            ICurrentTenantService tenant,
            IUnitOfWork uow,
            IProductRepository productrepo,
            IStockMovementRepository stockmovementrepo,
            IProductIngredientRepository productingredientrepo,
            ILogger<OrdersService> logger,
            INotificationsService notificationsService
            )
        {
            _orderrepo = orderrepo;
            _tenant = tenant;
            _uow = uow;
            _productrepo = productrepo;
            _stockmovementrepo = stockmovementrepo;
            _productingredientrepo = productingredientrepo;
            _logger = logger;
            _notificationsService = notificationsService;
        }
        public async Task<Guid> CreateOrderAsync(
    CreateOrderDto dto,
    CancellationToken cancellationToken)
        {
            var tenantId =
                _tenant.TenantId
                ?? throw new UnauthorizedException("Tenant required");

            if (dto.Items == null || !dto.Items.Any())
                throw new Exception("Order must have at least one item");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CustomerId = dto.CustomerId,
                OrderType = dto.OrderType,
                OrderStatus = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Unpaid,
                OrderNumber = await _orderrepo.GenerateOrderNumber(),
                Notes = dto.Notes,
                CreatedBy = tenantId,
            };

            decimal subtotal = 0;

            var groupedItems = dto.Items
                .GroupBy(x => x.ProductId);

            foreach (var group in groupedItems)
            {
                var product = await _productrepo
                    .GetByIdAsync(
                        group.Key,
                        cancellationToken);

                if (product == null)
                    throw new Exception("Product not found");

                if (product.TenantId != tenantId)
                    throw new Exception("Cross tenant product");

                if (!product.IsAvailable)
                    throw new Exception("Product unavailable");

                var qty = group.Sum(x => x.Quantity);

                var item = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    ProductNameSnapshot = product.ProductName,
                    UnitPrice = product.SellingPrice,
                    Quantity = qty,
                    LineTotal = qty * product.SellingPrice,
                    CreatedAt = DateTime.UtcNow,
                    TenantId = tenantId,
                };

                subtotal += (decimal)item.LineTotal;

                order.OrderItems.Add(item);
            }

            order.Subtotal = subtotal;
            order.TotalAmount = subtotal;

            await _orderrepo.AddAsync(order, cancellationToken);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);

				// FR-03: New Order notification
				try
				{
					await _notificationsService.SendNewOrderAlertAsync(order, tenantId, cancellationToken);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to send new order notification for {OrderId}", order.Id);
					// Don't rethrow — notification failure must not break order creation
				}
				return order.Id;
            }
            catch (DbUpdateException ex)
            {
                // هذا سيعطيك الـ inner exception الحقيقي
                var innerException = ex.InnerException?.Message;
                // سجل الـ inner exception
                _logger.LogError(ex, "Database error: {InnerException}", innerException);
                throw;
            }

        }


        public async Task<OrderListDto?> GetDetailsAsync(
    Guid orderId,
    CancellationToken cancellationToken)
        {
            var tenantId =
                _tenant.TenantId
                ?? throw new UnauthorizedException("Tenant required");

            var order =
                await _orderrepo.GetWithItemsAsync(
                    orderId,
                    tenantId,
                    cancellationToken);

            if (order == null)
                return null;

            return new OrderListDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderType = order.OrderType,
                OrderStatus = order.OrderStatus,
                PaymentStatus = order.PaymentStatus,
                Subtotal = order.Subtotal,
                TotalAmount = order.TotalAmount,

                Items = order.OrderItems.Select(x =>
                    new OrderItemDto
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        ProductName = x.ProductNameSnapshot,
                        Quantity = x.Quantity,
                        UnitPrice = x.UnitPrice,
                        LineTotal = x.LineTotal
                    })
                    .ToList()
            };
        }

        private async Task<List<InventoryItem>> CompleteOrderAsync(
    Order order,
    Guid tenantId,
    CancellationToken cancellationToken)
        {
            var productIds =
                order.OrderItems
                    .Select(x => x.ProductId)
                    .Distinct()
                    .ToList();

            var ingredients =
                await _productingredientrepo
                    .GetIngredientsByProductIdsAsync(
                        productIds,
                        tenantId,
                        cancellationToken);

            foreach (var orderItem in order.OrderItems)
            {
                var productIngredients =
                    ingredients
                        .Where(x =>
                            x.ProductId ==
                            orderItem.ProductId);

                foreach (var ingredient in productIngredients)
                {
                    var requiredQty =
                        ingredient.QuantityRequired
                        * orderItem.Quantity;

                    if (ingredient.InventoryItem.CurrentQuantity
                        < requiredQty)
                    {
                        throw new Exception(
                            $"Insufficient stock for {ingredient.InventoryItem.ItemName}");
                    }
                }
            }

			var affectedItems = new List<InventoryItem>();

			foreach (var orderItem in order.OrderItems)
            {
                var productIngredients =
                    ingredients
                        .Where(x =>
                            x.ProductId ==
                            orderItem.ProductId);

                foreach (var ingredient in productIngredients)
                {
                    var requiredQty =
                        ingredient.QuantityRequired
                        * orderItem.Quantity;

                    ingredient.InventoryItem.CurrentQuantity
                        -= (decimal)requiredQty;

					if (!affectedItems.Any(x => x.Id == ingredient.InventoryItem.Id))
					{
						affectedItems.Add(ingredient.InventoryItem);
					}

					await _stockmovementrepo.AddAsync(
                        new StockMovement
                        {
                            Id = Guid.NewGuid(),

                            TenantId = tenantId,

                            InventoryItemId =
                                ingredient.InventoryItemId,

                            TransactionType =
                                TransactionType.StockOut,

                            Quantity = (decimal)requiredQty,

                            TransactionDate =
                                DateTime.UtcNow,

                            Note =
                                $"Order {order.OrderNumber}"
                        },
                        cancellationToken);
                }
            }

            order.OrderStatus =
                OrderStatus.Completed;


			return affectedItems;
		}

        public async Task UpdateStatusAsync(
    Guid orderId,
    UpdateOrderStatusDto dto,
    CancellationToken cancellationToken)
        {
            var tenantId =
                _tenant.TenantId
                ?? throw new UnauthorizedException("Tenant required");

            var order =
                await _orderrepo.GetWithItemsAsync(
                    orderId,
                    tenantId,
                    cancellationToken);

            if (order == null)
                throw new Exception("Order not found");

            if (order.OrderStatus != OrderStatus.Pending)
                throw new Exception("Order is locked");

			List<InventoryItem>? affectedItems = null;


			switch (dto.Status)
            {
                case OrderStatus.Completed:

                    affectedItems = await CompleteOrderAsync(
                        order,
                        tenantId,
                        cancellationToken);

                    break;

                case OrderStatus.Cancelled:

                    order.OrderStatus =
                        OrderStatus.Cancelled;

                    break;

                default:

                    throw new Exception(
                        "Invalid status transition");
            }

            await _uow.SaveChangesAsync(
                cancellationToken);


			// FR-01 & FR-02: Stock alerts triggered only after transaction successfully commits
			if (affectedItems != null)
			{
				foreach (var item in affectedItems)
				{
					try
					{
						if (item.CurrentQuantity == 0)
						{
							await _notificationsService.SendOutOfStockAlertAsync(item, tenantId, cancellationToken);
						}
						else if (item.CurrentQuantity <= item.MinimumQuantity)
						{
							await _notificationsService.SendLowStockAlertAsync(item, tenantId, cancellationToken);
						}
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Failed to send stock alert for item {ItemId} after order completion", item.Id);
					}
				}
			}
		}

        public async Task UpdatePaymentStatusAsync(
    Guid orderId,
    UpdatePaymentStatusDto dto,
    CancellationToken cancellationToken)
        {
            var tenantId =
                _tenant.TenantId
                ?? throw new UnauthorizedException("Tenant required");

            var order =
                await _orderrepo.GetByIdAsync(
                    orderId,
                    tenantId,
                    cancellationToken);

            if (order == null)
                throw new Exception("Order not found");

            order.PaymentStatus =
                dto.Status;
            
            _orderrepo.Update(order);

            await _uow.SaveChangesAsync(
                cancellationToken);
        }



        public async Task<List<OrderListDto>> GetOrdersAsync(
        string? search,
        OrderStatus? status,
        PaymentStatus? paymentStatus,
        OrderType? orderType,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken)
        {
            var tenantId =
                _tenant.TenantId
                ?? throw new UnauthorizedException("Tenant required");

            return await _orderrepo.GetOrdersAsync(
                tenantId,
                search,
                status,
                paymentStatus,
                orderType,
                fromDate,
                toDate,
                cancellationToken);

        }
    }
}
