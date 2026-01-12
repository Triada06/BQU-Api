using System.Security.Claims;
using BGU.Api.Helpers;
using BGU.Application.Services;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Dean")]
public class DeanController(IDeanService deanService): ControllerBase
{
    [HttpGet(ApiEndPoints.Dean.Profile)]
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return Unauthorized();
        
        var res = await deanService.GetProfile(userId);
        return Ok(res);
    }
}