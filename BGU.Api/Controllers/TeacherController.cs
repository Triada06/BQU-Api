using System.Security.Claims;
using BGU.Api.Helpers;
using BGU.Application.Contracts.Teacher.Requests;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Teacher")]
public class TeacherController(ITeacherService teacherService) : ControllerBase
{
    [HttpGet(ApiEndPoints.Teacher.Profile)]
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var res = await teacherService.GetProfile(userId);
        return Ok(res);
    }

    [HttpGet(ApiEndPoints.Teacher.Schedule)]
    public async Task<IActionResult> Schedule(string schedule)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var res = await teacherService.GetSchedule(new TeacherScheduleRequest(userId, schedule));
        return Ok(res); //todo add classes and test it
    }

    [HttpGet(ApiEndPoints.Teacher.MyCourses)]
    public async Task<IActionResult> MyCourses()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return Unauthorized();
        var res = await teacherService.GetCourses(userId);
        return Ok(res); //todo add classes and test it
    }
}