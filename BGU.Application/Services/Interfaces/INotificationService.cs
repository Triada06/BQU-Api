using BGU.Application.Common;
using BGU.Application.Contracts.Notification.Requests;
using BGU.Application.Contracts.Notification.Responses;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Services.Interfaces;

public interface INotificationService
{
    Task<ApiResult> SendAsync(SendNotificationRequest request);
    Task<ApiResult> SendGroupNotificationAsync(string fromId, string groupId, SendNotificationToGroupRequest request);
    Task<ApiResult> SentToAllAsync(string fromId, SendToAllNotificationRequest request);
    Task<ApiResult> MarkAsReadAsync(string id);
    Task<ApiResult> MarkAllAsReadAsync(string userId);
    Task<ApiResult<PagedResponse<NotificationResponseDto>>> GetAllByUserId(string userId, int page, int pageSize);
}