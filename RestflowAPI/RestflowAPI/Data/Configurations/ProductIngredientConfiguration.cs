using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestflowAPI.Entities;

namespace RestflowAPI.Data.Configurations;

public class ProductIngredientConfiguration : IEntityTypeConfiguration<ProductIngredient>
{
    public void Configure(EntityTypeBuilder<ProductIngredient> builder)
    {
        builder.ToTable("ProductIngredients");
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.QuantityRequired).HasPrecision(18, 4);
        builder.Property(p => p.UnitOfMeasure).IsRequired().HasMaxLength(50);
        
        builder.HasOne(p => p.Tenant)
            .WithMany(t => t.ProductIngredients)
            .HasForeignKey(p => p.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(p => p.Product)
            .WithMany(pr => pr.ProductIngredients)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(p => p.InventoryItem)
            .WithMany(i => i.ProductIngredients)
            .HasForeignKey(p => p.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasQueryFilter(p => p.DeletedAt == null);
    }
}
