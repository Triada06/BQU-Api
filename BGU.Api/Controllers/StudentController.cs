using System.Security.Claims;
using BGU.Api.Helpers;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Student,Teacher,Dean")]
public class StudentController(IStudentService studentService) : ControllerBase
{
    [HttpGet(ApiEndPoints.Student.DashBoard)]
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var data = await studentService.Profile(userId);
        return Ok(data);
    }
}