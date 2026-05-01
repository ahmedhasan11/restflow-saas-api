using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestflowAPI.Entities;

namespace RestflowAPI.Data.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("InventoryItems");
        builder.HasKey(i => i.Id);
        
        builder.Property(i => i.ItemName).IsRequired().HasMaxLength(150);
        builder.Property(i => i.UnitOfMeasure).IsRequired().HasMaxLength(50);
        builder.Property(i => i.CurrentQuantity).HasPrecision(18, 4);
        builder.Property(i => i.MinimumQuantity).HasPrecision(18, 4);
        builder.Property(i => i.CostPerUnit).HasPrecision(18, 4);
        
        builder.Property(i => i.RowVersion).IsRowVersion();
        
        builder.HasOne(i => i.Tenant)
            .WithMany(t => t.InventoryItems)
            .HasForeignKey(i => i.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(i => i.Category)
            .WithMany(c => c.InventoryItems)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasQueryFilter(i => i.DeletedAt == null);
    }
}
