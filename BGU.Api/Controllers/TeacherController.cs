using System.Security.Claims;
using BGU.Api.Helpers;
using BGU.Application.Contracts.Teacher.Requests;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
public class TeacherController(ITeacherService teacherService) : ControllerBase
{
    [Authorize(Roles = "Dean")]
    [HttpGet(ApiEndPoints.Teacher.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var res = await teacherService.GetAllAsync(page, pageSize);
        return new ObjectResult(res);
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpGet(ApiEndPoints.Teacher.Profile)]
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var res = await teacherService.GetProfile(userId);
        return Ok(res);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet(ApiEndPoints.Teacher.Schedule)]
    public async Task<IActionResult> Schedule(string schedule)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var res = await teacherService.GetSchedule(new TeacherScheduleRequest(userId, schedule));
        return Ok(res); 
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet(ApiEndPoints.Teacher.MyCourses)]
    public async Task<IActionResult> MyCourses()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return Unauthorized();
        var res = await teacherService.GetCourses(userId);
        return Ok(res); 
    }
    //
    // [Authorize(Roles = "Dean")]
    // [HttpPut(ApiEndPoints.Teacher.Update)]
    // public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateTeacherRequest request)
    // {
    //     var res = await teacherService.UpdateAsync(id, request);
    //     return new ObjectResult(res);
    // } 
    //
    // [Authorize(Roles = "Dean")]
    // [HttpDelete(ApiEndPoints.Teacher.Delete)]
    // public async Task<IActionResult> Delete([FromRoute] string id)
    // {
    //     var res = await teacherService.DeleteAsync(id);
    //     return new ObjectResult(res);
    // } 
    //
    // [Authorize(Roles = "Dean")]
    // [HttpGet(ApiEndPoints.Teacher.GetById)]
    // public async Task<IActionResult> GetById([FromRoute] string id)
    // {
    //     var res = await teacherService.GetByIdAsync(id);
    //     return new ObjectResult(res);
    // } 
    
}