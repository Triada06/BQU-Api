using BGU.Api.Helpers;
using BGU.Application.Contracts.Group.Requests;
using BGU.Application.Dtos.Exams;
using BGU.Application.Services;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Dean")]
public class GroupController(IGroupService groupService, IFinalService finalService) : ControllerBase
{
    [HttpGet(ApiEndPoints.Group.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var res = await groupService.GetAllAsync(page, pageSize);
        return new ObjectResult(res);
    }

    [HttpGet(ApiEndPoints.Group.GetById)]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var res = await groupService.GetByIdAsync(id);
        return new ObjectResult(res);
    }

    [HttpGet(ApiEndPoints.Group.Schedule)]
    public async Task<IActionResult> Schedule(string id)
    {
        var data = await groupService.GetSchedule(id);
        return Ok(data);
    }

    [HttpDelete(ApiEndPoints.Group.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var res = await groupService.DeleteAsync(id);
        return new ObjectResult(res);
    }

    [HttpPut(ApiEndPoints.Group.Update)]
    public async Task<IActionResult> Update([FromRoute] string id, UpdateGroupRequest request)
    {
        var res = await groupService.UpdateAsync(id, request);
        return new ObjectResult(res);
    }

    [HttpPost(ApiEndPoints.Group.Create)]
    public async Task<IActionResult> Create(CreateGroupRequest request)
    {
        var res = await groupService.CreateAsync(request);
        return new ObjectResult(res);
    }

    [HttpPut(ApiEndPoints.Group.SetExamDate)]
    public async Task<IActionResult> SetExamDate([FromBody] SetGroupExamDto request)
    {
        var res = await finalService.SetGroupExamDateAsync(request);
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }
}