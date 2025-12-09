using BGU.Api.Helpers;
using BGU.Application.Contracts.Group.Requests;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Dean")]
public class GroupController(IGroupService groupService) : ControllerBase
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
}