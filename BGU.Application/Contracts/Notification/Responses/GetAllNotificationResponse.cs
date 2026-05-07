using BGU.Core.Enums;

namespace BGU.Application.Contracts.Notification.Responses;

public sealed record GetAllNotificationResponse(IEnumerable<NotificationResponseDto> Notifications);

public sealed record NotificationResponseDto(
    string Id,
    string FromFullName,
    string Message,
    string ReceiveDate,
    NotificationType NotificationType,
    bool IsRead);