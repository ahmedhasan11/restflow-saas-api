using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestflowAPI.Entities;

namespace RestflowAPI.Data.Configurations;

public class MenuCategoryConfiguration : IEntityTypeConfiguration<MenuCategory>
{
    public void Configure(EntityTypeBuilder<MenuCategory> builder)
    {
        builder.ToTable("MenuCategories");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.CategoryName).IsRequired().HasMaxLength(100);
        
        builder.HasOne(c => c.Tenant)
            .WithMany(t => t.MenuCategories)
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasQueryFilter(c => c.DeletedAt == null);
    }
}
