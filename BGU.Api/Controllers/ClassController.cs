using BGU.Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
public class ClassController : ControllerBase
{
    [HttpGet(ApiEndPoints.Class.GetAll)]
    public async Task<IActionResult> GetAll(string userId)
    {
        return Ok();
    }
}