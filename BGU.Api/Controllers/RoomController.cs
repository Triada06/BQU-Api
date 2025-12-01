using BGU.Api.Helpers;
using BGU.Application.Contracts.Rooms.Requests;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Dean")]
public class RoomController(IRoomService roomService) : ControllerBase
{
    [HttpGet(ApiEndPoints.Room.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // if (userId is null)
        //     return Unauthorized();
        var res = await roomService.GetAllAsync(page, pageSize);
        return new ObjectResult(res);
    }

    [HttpPost(ApiEndPoints.Room.Create)]
    public async Task<IActionResult> Create([FromBody] CreateRoomRequest request)
    {
        var res = await roomService.CreateAsync(request);
        return new ObjectResult(res);
    }

    [HttpGet(ApiEndPoints.Room.GetById)]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var res = await roomService.GetByIdAsync(id);
        return new ObjectResult(res);
    }

    [HttpPut(ApiEndPoints.Room.Update)]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateRoomRequest request)
    {
        var res = await roomService.UpdateAsync(id,request);
        return new ObjectResult(res);
    }

    [HttpDelete(ApiEndPoints.Room.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var res = await roomService.DeleteAsync(id);
        return new ObjectResult(res);
    }
}