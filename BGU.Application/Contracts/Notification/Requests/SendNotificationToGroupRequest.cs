using BGU.Core.Enums;

namespace BGU.Application.Contracts.Notification.Requests;

public sealed record SendNotificationToGroupRequest(NotificationType NotificationType, string Message);