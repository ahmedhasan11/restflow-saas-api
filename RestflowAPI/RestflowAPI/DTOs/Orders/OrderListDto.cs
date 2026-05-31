using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.Orders
{
    public class OrderListDto
    {
        public Guid Id { get; set; }

        public string? OrderNumber { get; set; }

        public OrderType? OrderType { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }

        public decimal? Subtotal { get; set; }

        public decimal? TotalAmount { get; set; }

        public List<OrderItemDto> Items { get; set; }
            = new();
    }
}

