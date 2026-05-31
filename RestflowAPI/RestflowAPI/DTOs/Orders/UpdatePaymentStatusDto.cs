using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.Orders
{
    public class UpdatePaymentStatusDto
    {
        public PaymentStatus? Status { get; set; }
    }
}
