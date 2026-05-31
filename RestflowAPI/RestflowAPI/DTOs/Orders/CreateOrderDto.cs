using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.Orders
{
    public class CreateOrderDto
    {
        public Guid? CustomerId { get; set; }

        public OrderType? OrderType { get; set; }

        public string? Notes { get; set; }

        public List<CreateOrderItemDto> Items { get; set; }
            = new();
    }
}
