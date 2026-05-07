using BGU.Core.Enums;

namespace BGU.Core.Entities;

public class Notification : BaseEntity
{
    public bool IsRead { get; set; }
    public required NotificationType Type { get; init; }
    public required string From { get; set; }
    public required string To { get; set; }
    public required string Message { get; set; }
}