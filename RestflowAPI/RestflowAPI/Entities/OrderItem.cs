using RestflowAPI.ServiceInterfaces.Tenants;

namespace RestflowAPI.Entities
{
    public class OrderItem : BaseEntity, IMustHaveTenant
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }
        public Guid TenantId { get; set; }

        public string ProductNameSnapshot { get; set; } = string.Empty;

        public decimal? UnitPrice { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? LineTotal { get; set; }

        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public Tenant Tenant { get; set; } = null!;
    }
}
