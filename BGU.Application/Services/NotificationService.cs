using BGU.Application.Common;
using BGU.Application.Contracts.Notification.Requests;
using BGU.Application.Contracts.Notification.Responses;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class NotificationService(INotificationRepository notificationRepository, UserManager<AppUser> userManager)
    : INotificationService
{
    public async Task<ApiResult> SendAsync(SendNotificationRequest request)
    {
        if (!request.From.Trim().Equals("system", StringComparison.CurrentCultureIgnoreCase))
        {
            if (!await EnsureUserExists(request.From))
            {
                return ApiResult.NotFound($"User with an id of {request.From} does not exist.");
            }
        }

        if (!await EnsureUserExists(request.To))
        {
            return ApiResult.NotFound($"User with an id of {request.To} does not exist.");
        }

        var notification = new Notification
        {
            Type = request.NotificationType,
            From = request.From,
            To = request.To,
            Message = request.Message
        };

        var res = await notificationRepository.CreateAsync(notification);

        return res ? ApiResult.Success() : ApiResult.SystemError();
    }

    public async Task<ApiResult> MarkAsReadAsync(string id)
    {
        var notification = await notificationRepository.GetByIdAsync(id);
        if (notification == null)
        {
            return ApiResult.NotFound($"Notification with id {id} not found.");
        }

        notification.IsRead = true;
        var res = await notificationRepository.UpdateAsync(notification);

        return res ? ApiResult.Success() : ApiResult.SystemError();
    }

    public async Task<ApiResult> MarkAllAsReadAsync(string userId)
    {
        if (!await EnsureUserExists(userId))
        {
            return ApiResult.NotFound($"User with an id of {userId} does not exist.");
        }

        var notifications = await notificationRepository.FindAsync(x => x.To == userId && !x.IsRead, tracking: true);

        if (notifications.Count == 0)
        {
            return ApiResult.Success();
        }

        if (!await notificationRepository.BulkMarkAllAsReadAsync(notifications.Select(x => x.Id)))
        {
            return ApiResult.SystemError("Something went wrong wile saving changes");
        }

        return ApiResult.Success();
    }

    public async Task<ApiResult<GetAllNotificationResponse>> GetAllByUserId(string userId)
    {
        if (!await EnsureUserExists(userId))
        {
            return ApiResult<GetAllNotificationResponse>.NotFound($"User with an id of {userId} does not exist.");
        }

        var notifications = await notificationRepository.FindAsync(x => x.To == userId, tracking: false);

        if (notifications.Count == 0)
        {
            return ApiResult<GetAllNotificationResponse>.Success(
                new GetAllNotificationResponse(new List<NotificationResponseDto>()));
        }

        notifications = notifications.OrderByDescending(x => x.CreatedAt).ToList();

        var fromIds = notifications.Select(x => x.From).Distinct().ToList();
        var users = await userManager.Users.Where(x => fromIds.Contains(x.Id)).ToListAsync();

        IList<NotificationResponseDto> notificationResponses = new List<NotificationResponseDto>();

        foreach (var notification in notifications)
        {
            var user = users.Find(x => x.Id == notification.From);
            var userName = user is null ? "System" : user.Name + " " + user.Surname + " " + user.MiddleName;

            notificationResponses.Add(new NotificationResponseDto(notification.Id, userName, notification.Message,
                notification.CreatedAt.ToString("dddd, MMM dd"), notification.Type, notification.IsRead));
        }

        return ApiResult<GetAllNotificationResponse>.Success(
            new GetAllNotificationResponse(notificationResponses));
    }


    private async Task<bool> EnsureUserExists(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        return user != null;
    }
}