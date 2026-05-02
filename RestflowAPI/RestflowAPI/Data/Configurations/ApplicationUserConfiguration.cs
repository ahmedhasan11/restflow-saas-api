using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestflowAPI.Entities;

namespace RestflowAPI.Data.Configurations
{
	public class ApplicationUserConfiguration:IEntityTypeConfiguration<ApplicationUser>
	{
		public void Configure(EntityTypeBuilder<ApplicationUser> builder)
		{
			builder.ToTable("Users");

			builder.Property(u => u.FullName).IsRequired().HasMaxLength(150);
			builder.Property(u => u.Status).IsRequired().HasConversion<string>();

			builder.HasOne(u => u.Tenant)
				.WithMany(t => t.Users)
				.HasForeignKey(u => u.TenantId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasQueryFilter(u => u.DeletedAt == null);
		}
	}
}
