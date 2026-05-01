using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestflowAPI.Entities;

namespace RestflowAPI.Data.Configurations;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements");
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.TransactionType).IsRequired().HasConversion<string>();
        builder.Property(s => s.Quantity).HasPrecision(18, 4);
        builder.Property(s => s.UnitCost).HasPrecision(18, 4);
        builder.Property(s => s.TotalCost).HasPrecision(18, 4);
        
        builder.HasOne(s => s.Tenant)
            .WithMany(t => t.StockMovements)
            .HasForeignKey(s => s.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(s => s.InventoryItem)
            .WithMany(i => i.StockMovements)
            .HasForeignKey(s => s.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasQueryFilter(s => s.DeletedAt == null);
    }
}
