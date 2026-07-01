using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestflowAPI.Entities;

namespace RestflowAPI.Data.Configurations
{
	public class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
	{
		public void Configure(EntityTypeBuilder<DeviceToken> builder)
		{
			builder.ToTable("DeviceTokens");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.Token)
				.IsRequired()
				.HasMaxLength(1000);

			builder.Property(x => x.DeviceType)
				.IsRequired()
				.HasConversion<string>();

			builder.HasOne(x => x.User)
				.WithMany()
				.HasForeignKey(x => x.UserId)
				.OnDelete(DeleteBehavior.Cascade);


			// Unique index on Token
			builder.HasIndex(x => x.Token)
				.IsUnique();
		}
	}
}
