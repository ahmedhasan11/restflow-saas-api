using System;
using RestflowAPI.Enums;

namespace RestflowAPI.Entities;

public class OtpVerification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ChannelType ChannelType { get; set; }
    public string OtpCodeHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public int ResendCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public User User { get; set; } = null!;
}
