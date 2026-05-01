using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestflowAPI.Entities;

namespace RestflowAPI.Data.Configurations;

public class InventoryCategoryConfiguration : IEntityTypeConfiguration<InventoryCategory>
{
    public void Configure(EntityTypeBuilder<InventoryCategory> builder)
    {
        builder.ToTable("InventoryCategories");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.CategoryName).IsRequired().HasMaxLength(100);
        
        builder.HasQueryFilter(c => c.DeletedAt == null);
    }
}
