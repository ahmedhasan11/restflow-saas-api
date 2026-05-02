using Microsoft.AspNetCore.Identity;
using RestflowAPI.Enums;

namespace RestflowAPI.Entities
{
	public class ApplicationUser:IdentityUser<Guid>, IAuditable
	{
		public Guid? TenantId { get; set; }
		public string FullName { get; set; } = string.Empty;

		// We remove Phone/Email/PasswordHash because IdentityUser already has PhoneNumber, Email, and PasswordHash
		// We also map EmailVerified and PhoneVerified to IdentityUser's EmailConfirmed and PhoneNumberConfirmed

		public UserStatus Status { get; set; } = UserStatus.Active;
		public string? ProfileImageUrl { get; set; }
		public string PreferredLanguage { get; set; } = "en";
		public string? NotificationPreferences { get; set; }
		public DateTime? LastLoginAt { get; set; }

		// Audit and Soft Delete fields normally in BaseEntity
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; }
		public Guid? CreatedBy { get; set; }
		public Guid? UpdatedBy { get; set; }
		public DateTime? DeletedAt { get; set; }

		// Navigation Properties
		public Tenant? Tenant { get; set; }
		public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
		public ICollection<OtpVerification> OtpVerifications { get; set; } = new List<OtpVerification>();
	}
}
