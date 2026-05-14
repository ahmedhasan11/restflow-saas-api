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

		builder.Property(t => t.Country).IsRequired().HasMaxLength(100);
		builder.Property(t => t.DefaultLanguage).IsRequired().HasMaxLength(5).HasDefaultValue("en");
		builder.Property(t => t.Timezone).IsRequired().HasMaxLength(50).HasDefaultValue("UTC");
		builder.Property(t => t.Currency).IsRequired().HasMaxLength(3).HasDefaultValue("USD");

		builder.HasQueryFilter(t => t.DeletedAt == null);
    }
}
