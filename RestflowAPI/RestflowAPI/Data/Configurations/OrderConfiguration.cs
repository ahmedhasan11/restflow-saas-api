using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestflowAPI.Entities;

namespace RestflowAPI.Data.Configurations
{
    

    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Subtotal)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.HasMany(x => x.OrderItems)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.Customer)
                    .WithMany(x=>x.orders)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(i => i.Tenant)
                   .WithMany(x=>x.Orders)
                   .HasForeignKey(o => o.TenantId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(c => c.DeletedAt == null);
        }
    }
}
