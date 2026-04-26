using BGU.Api.Helpers;
using BGU.Application.Contracts.ClassTime.Requests;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Teacher,Dean")]
public class ClassTimeController(IClassTimeService classTimeService) : ControllerBase
{
    [HttpPost(ApiEndPoints.ClassTime.Create)]
    public async Task<IActionResult> Create([FromBody] CreateClassTimeRequest request)
    {
        var res = await classTimeService.CreateAsync(request);
        return new ObjectResult(res);
    }
}