namespace RestflowAPI.DTOs.Orders
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public string? ProductName { get; set; } = string.Empty;
        public string? ProductNameSnapshot { get; set; } = string.Empty;

        public decimal? UnitPrice { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? LineTotal { get; set; }
    }
}
