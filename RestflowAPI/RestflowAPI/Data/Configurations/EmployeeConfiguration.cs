using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestflowAPI.Entities;

namespace RestflowAPI.Data.Configurations
{
	public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
	{
		public void Configure(EntityTypeBuilder<Employee> builder)
		{
			builder.ToTable("Employees");
			builder.HasKey(e => e.Id);

			builder.Property(e => e.FullName).IsRequired().HasMaxLength(150);
			builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
			builder.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(50);
			builder.Property(e => e.Role).IsRequired().HasMaxLength(50);
			builder.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(20);

			// Indexes for fast lookups and uniqueness within the tenant or globally
			builder.HasIndex(e => e.Email).IsUnique();
			builder.HasIndex(e => e.PhoneNumber).IsUnique();

			// 1-to-Many Tenant -> Employees
			builder.HasOne(e => e.Tenant)
				.WithMany()
				.HasForeignKey(e => e.TenantId)
				.OnDelete(DeleteBehavior.Restrict);

			// 1-to-1 ApplicationUser -> Employee
			builder.HasOne(e => e.User)
				.WithOne(u => u.Employee)
				.HasForeignKey<Employee>(e => e.UserId)
				.OnDelete(DeleteBehavior.SetNull);

			// Global Query Filter for soft delete
			builder.HasQueryFilter(e => e.DeletedAt == null);
		}
	}
}
