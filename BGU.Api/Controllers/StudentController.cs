using System.Security.Claims;
using BGU.Api.Helpers;
using BGU.Application.Contracts.Student.Requests;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Student")]
public class StudentController(IStudentService studentService) : ControllerBase
{
    [HttpGet(ApiEndPoints.Student.DashBoard)]
    public async Task<IActionResult> Dashboard()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var data = await studentService.Dashboard(userId);
        return Ok(data);
    }

    [HttpGet(ApiEndPoints.Student.Schedule)]
    public async Task<IActionResult> Schedule(string schedule)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var data = await studentService.GetSchedule(userId, new StudentScheduleRequest(schedule));
        return Ok(data);
    }

    [HttpGet(ApiEndPoints.Student.Grades)]
    public async Task<IActionResult> Grades(string grade) //todo: test this feat with data
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var data = await studentService.GetGrades(userId, new StudentGradesRequest(grade));
        return Ok(data);
    }

    [HttpGet(ApiEndPoints.Student.Profile)]
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var data = await studentService.GetProfile(userId);
        return Ok(data);
    }

    [Authorize(Roles = "Dean")]
    [HttpGet(ApiEndPoints.Student.Filter)]
    public async Task<IActionResult> FilterBy([FromQuery] string groupId, [FromQuery] int year)
    {
        var res = await studentService.FilterAsync(groupId, year);
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean")]
    [HttpGet(ApiEndPoints.Student.Search)]
    public async Task<IActionResult> Search([FromQuery] string searchText)
    {
        var res = await studentService.SearchAsync(searchText);
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean")]
    [HttpGet(ApiEndPoints.Student.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var res = await studentService.GetAllAsync(page, pageSize);
        return new ObjectResult(res);
    }
}