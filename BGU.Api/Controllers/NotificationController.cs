using System.Security.Claims;
using BGU.Api.Helpers;
using BGU.Application.Contracts.Notification.Requests;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
public class NotificationController(INotificationService notificationService) : ControllerBase
{
    [Authorize(Roles = "Teacher, Dean")]
    [HttpPost(ApiEndPoints.Notification.Create)]
    public async Task<IActionResult> Create([FromBody] SendNotificationRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var res = await notificationService.SendAsync(request with { From = userId });
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Student, Teacher, Dean")]
    [HttpPut(ApiEndPoints.Notification.MarkAsRead)]
    public async Task<IActionResult> MarkAsRead([FromRoute] string id)
    {
        var res = await notificationService.MarkAsReadAsync(id);
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Student, Teacher, Dean")]
    [HttpGet(ApiEndPoints.User.GetNotifications)]
    public async Task<IActionResult> GetNotifications()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var res = await notificationService.GetAllByUserId(userId);
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Student, Teacher, Dean")]
    [HttpPut(ApiEndPoints.User.MarkAllAsRead)]
    public async Task<IActionResult> MarkAsRead()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var res = await notificationService.MarkAllAsReadAsync(userId);
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Teacher, Dean")]
    [HttpPost(ApiEndPoints.Group.SendNotification)]
    public async Task<IActionResult> SendGroupNotification([FromRoute] string id,
        [FromBody] SendNotificationToGroupRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var res = await notificationService.SendGroupNotificationAsync(userId, id, request);
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean")]
    [HttpPost(ApiEndPoints.User.SendNotificationToAll)]
    public async Task<IActionResult> SentToAll([FromBody] SendToAllNotificationRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var res = await notificationService.SentToAllAsync(userId, request);
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }
}