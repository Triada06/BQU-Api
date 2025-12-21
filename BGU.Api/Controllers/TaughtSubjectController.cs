using BGU.Api.Helpers;
using BGU.Application.Contracts.TaughtSubjects.Requests;
using BGU.Application.Dtos.TaughtSubject.Requests;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Dean")]
//todo: test the whole controller
public class TaughtSubjectController(ITaughtSubjectService taughtSubjectService) : ControllerBase
{
    [HttpGet(ApiEndPoints.TaughtSubject.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var res = await taughtSubjectService.GetAllAsync(page, pageSize);
        return new ObjectResult(res);
    }

    [HttpGet(ApiEndPoints.TaughtSubject.GetById)]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var res = await taughtSubjectService.GetByIdAsync(id);
        return new ObjectResult(res);
    }

    [HttpGet(ApiEndPoints.TaughtSubject.Update)]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateTaughtSubjectRequest request)
    {
        var res = await taughtSubjectService.UpdateAsync(id, request);
        return new ObjectResult(res);
    }

    [HttpGet(ApiEndPoints.TaughtSubject.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var res = await taughtSubjectService.DeleteAsync(id);
        return new ObjectResult(res);
    }
    [HttpPost(ApiEndPoints.TaughtSubject.Create)]
    public async Task<IActionResult> Create([FromBody] CreateTaughtSubjectRequest request)
    {
        var res = await taughtSubjectService.CreateAsync(request);
        return new ObjectResult(res);
    }
}