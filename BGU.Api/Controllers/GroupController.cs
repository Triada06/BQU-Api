using BGU.Api.Helpers;
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
}