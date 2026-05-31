namespace RestflowAPI.DTOs.Orders
{
    public class CreateOrderItemDto
    {
        public Guid ProductId { get; set; }

        public decimal? Quantity { get; set; }
    }
}
