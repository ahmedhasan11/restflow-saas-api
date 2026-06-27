using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestflowAPI.Entities;

namespace RestflowAPI.Data.Configurations
{
	public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
	{
		public void Configure(EntityTypeBuilder<Notification> builder)
		{
			builder.ToTable("Notifications");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.Title)
				.IsRequired()
				.HasMaxLength(200);

			builder.Property(x => x.Body)
				.IsRequired()
				.HasMaxLength(1000);

			builder.Property(x => x.Type)
				.IsRequired()
				.HasConversion<string>()
				.HasMaxLength(20);

			builder.HasOne(x => x.User)
				.WithMany()
				.HasForeignKey(x => x.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasQueryFilter(x => x.DeletedAt == null);

			// Index for fast unread counts/queries
			builder.HasIndex(x => new { x.TenantId, x.UserId, x.ReadAt });
		}
	}
}
