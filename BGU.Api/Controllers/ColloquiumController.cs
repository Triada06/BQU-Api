using BGU.Api.Helpers;
using BGU.Application.Contracts.Colloquium.Requests;
using BGU.Application.Services;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Teacher")]
public class ColloquiumController(IColloquiumService colloquiumService) : ControllerBase
{
    [HttpPost(ApiEndPoints.Colloquium.Create)]
    public async Task<IActionResult> Create([FromBody] CreateColloquiumRequest request)
    {
        var res = await colloquiumService.CreateAsync(request);
        return Ok(res);
    }

    [HttpDelete(ApiEndPoints.Colloquium.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var res = await colloquiumService.DeleteAsync(id);
        return Ok(res);
    }
}