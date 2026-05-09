using BGU.Core.Enums;

namespace BGU.Application.Contracts.Notification.Requests;

public sealed record SendToAllNotificationRequest(NotificationType NotificationType, string Message);