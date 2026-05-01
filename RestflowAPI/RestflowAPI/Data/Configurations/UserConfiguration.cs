using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestflowAPI.Entities;

namespace RestflowAPI.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.HasIndex(u => u.Email).IsUnique();
        
        builder.Property(u => u.Phone).IsRequired().HasMaxLength(50);
        builder.HasIndex(u => u.Phone).IsUnique();
        
        builder.Property(u => u.Role).IsRequired().HasConversion<string>();
        builder.Property(u => u.Status).IsRequired().HasConversion<string>();
        
        builder.HasOne(u => u.Tenant)
            .WithMany(t => t.Users)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasQueryFilter(u => u.DeletedAt == null);
    }
}
