using BGU.Api.Helpers;
using BGU.Application.Contracts.IndependentWorks.Requests;
using BGU.Application.Dtos.IndependentWorks;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Teacher")]
public class IndependentWorkController(IIndependentWorkService independentWorkService) : ControllerBase
{
    [HttpPost(ApiEndPoints.IndependentWork.Create)]
    public async Task<IActionResult> Create([FromBody] GradeIndependentWorkRequest request)
    {
        var res = await independentWorkService.CreateAsync(request);
        return new OkObjectResult(res);
    }

    [HttpDelete(ApiEndPoints.IndependentWork.Delete)]
    public async Task<IActionResult> Create([FromRoute] string id)
    {
        var res = await independentWorkService.DeleteAsync(id);
        return new OkObjectResult(res);
    }

    [HttpPut(ApiEndPoints.IndependentWork.Grade)]
    public async Task<IActionResult> BulkGradeIndependentWork([FromRoute] string id,
        [FromBody] GradeIndependentWorkDto independentWork)
    {
        var res = await independentWorkService.BulkGradeIndependentWorkAsync(id,
            independentWork);
        return new ObjectResult(res);
    }
}