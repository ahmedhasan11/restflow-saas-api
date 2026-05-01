using System;

namespace RestflowAPI.Entities;

public class PlatformSetting : BaseEntity
{
    public Guid Id { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public bool IsSecret { get; set; }
}
