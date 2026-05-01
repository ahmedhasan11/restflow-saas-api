using System;
using System.Collections.Generic;
using RestflowAPI.Enums;

namespace RestflowAPI.Entities;

public class User : BaseEntity
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string PreferredLanguage { get; set; } = "en";
    public string? NotificationPreferences { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navigation Properties
    public Tenant? Tenant { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<OtpVerification> OtpVerifications { get; set; } = new List<OtpVerification>();
}
