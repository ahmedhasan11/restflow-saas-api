using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestflowAPI.Entities;

namespace RestflowAPI.Data.Configurations;

public class OtpVerificationConfiguration : IEntityTypeConfiguration<OtpVerification>
{
    public void Configure(EntityTypeBuilder<OtpVerification> builder)
    {
        builder.ToTable("OtpVerifications");
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.ChannelType).IsRequired().HasConversion<string>();
        builder.Property(o => o.OtpCodeHash).IsRequired().HasMaxLength(255);
        
        builder.HasOne(o => o.User)
            .WithMany(u => u.OtpVerifications)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasQueryFilter(o => o.DeletedAt == null);
    }
}
