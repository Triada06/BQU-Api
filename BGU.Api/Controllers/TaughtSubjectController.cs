using BGU.Api.Helpers;
using BGU.Application.Contracts.Colloquium.Requests;
using BGU.Application.Contracts.TaughtSubjects.Requests;
using BGU.Application.Dtos.TaughtSubject.Requests;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
public class TaughtSubjectController(ITaughtSubjectService taughtSubjectService, IColloquiumService colloquiumService)
    : ControllerBase
{
    [Authorize(Roles = "Dean")]
    [HttpGet(ApiEndPoints.TaughtSubject.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var res = await taughtSubjectService.GetAllAsync(page, pageSize);
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean")]
    [HttpGet(ApiEndPoints.TaughtSubject.GetById)]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var res = await taughtSubjectService.GetByIdAsync(id);
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean")]
    [HttpPut(ApiEndPoints.TaughtSubject.Update)]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateTaughtSubjectRequest request)
    {
        var res = await taughtSubjectService.UpdateAsync(id, request);
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean")]
    [HttpDelete(ApiEndPoints.TaughtSubject.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var res = await taughtSubjectService.DeleteAsync(id);
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean")]
    [HttpPost(ApiEndPoints.TaughtSubject.Create)]
    public async Task<IActionResult> Create([FromBody] CreateTaughtSubjectRequest request)
    {
        var res = await taughtSubjectService.CreateAsync(request);
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet(ApiEndPoints.TaughtSubject.GetAllColloquiums)]
    public async Task<IActionResult> GetAllColloquiums([FromRoute] string taughtSubjectId)
    {
        var res = await colloquiumService.GetAllAsync(taughtSubjectId);
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet(ApiEndPoints.TaughtSubject.GetStudentsAndAttendances)]
    public async Task<IActionResult> GetStudentsAndAttendances([FromRoute] string taughtSubjectId)
    {
        var res = await taughtSubjectService.GetStudentsAndAttendances(taughtSubjectId);
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet(ApiEndPoints.TaughtSubject.GetStudentsInSubject)] //dlya pokaza studntov 
    public async Task<IActionResult> GetStudentsInSubject([FromRoute] string taughtSubjectId)
    {
        var res = await taughtSubjectService.GetStudentsAsync(taughtSubjectId);
        return new ObjectResult(res);
    }
}