using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.Orders
{
    public class UpdateOrderStatusDto
    {
        public OrderStatus? Status { get; set; }
    }
}
