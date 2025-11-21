using System.Security.Claims;
using BGU.Api.Helpers;
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
        return Ok(res); //todo gives null temp password fix it
    }
}