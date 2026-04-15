using BGU.Api.Helpers;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Dean")]
public class FinalController(IFinalService finalService) : ControllerBase
{
    [HttpGet(ApiEndPoints.Finals.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var res = await finalService.GetAllAsync();
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }
}