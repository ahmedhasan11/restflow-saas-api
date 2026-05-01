using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestflowAPI.Entities;

namespace RestflowAPI.Data.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.TenantCode).IsRequired().HasMaxLength(50);
        builder.HasIndex(t => t.TenantCode).IsUnique();
        
        builder.Property(t => t.RestaurantName).IsRequired().HasMaxLength(150);
        builder.Property(t => t.Status).IsRequired().HasConversion<string>();
        
        builder.HasQueryFilter(t => t.DeletedAt == null);
    }
}
