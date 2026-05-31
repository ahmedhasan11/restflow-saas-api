using MailKit.Search;
using RestflowAPI.Enums;
using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Entities
{
    public class Order : BaseEntity, IMustHaveTenant
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid? CreatedBy { get; set; } 

        public string OrderNumber { get; set; } = string.Empty;

        public OrderType? OrderType { get; set; } 

        public OrderStatus? OrderStatus { get; set; } 

        public PaymentStatus? PaymentStatus { get; set; } 

        public decimal? Subtotal { get; set; } 

        public decimal? TotalAmount { get; set; } 

        public string? Notes { get; set; } = string.Empty!;

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();

        public Customer? Customer { get; set; } = null!;
        public Tenant? Tenant { get; set; } = null!;
    }
}
