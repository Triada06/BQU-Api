using BGU.Api.Helpers;
using BGU.Application.Contracts.Seminars.Requests;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Teacher")]
public class SeminarController(ISeminarService seminarService) : ControllerBase
{
    [HttpPost(ApiEndPoints.Seminar.Create)]
    public async Task<IActionResult> Create([FromRoute] string studentId, [FromRoute] string taughtSubjectId)
    {
        var res = await seminarService.CreateAsync(new CreateSeminarRequest(studentId, taughtSubjectId));
        return new ObjectResult(res);
    }
    [HttpDelete(ApiEndPoints.Seminar.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var res = await seminarService.DeleteAsync(id);
        return new ObjectResult(res);
    }
}