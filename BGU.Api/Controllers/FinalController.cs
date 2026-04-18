using System.Security.Claims;
using BGU.Api.Helpers;
using BGU.Application.Dtos.Exams;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
public class FinalController(IFinalService finalService) : ControllerBase
{
    [Authorize(Roles = "Dean")]
    [HttpGet(ApiEndPoints.Finals.GetAll)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var res = await finalService.GetAllAsync(page, pageSize, search);
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean")]
    [HttpPost(ApiEndPoints.Finals.Create)]
    public async Task<IActionResult> Create(CreateExamDto request)
    {
        var res = await finalService.CreateAsync(request);
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean")]
    [HttpPut(ApiEndPoints.Finals.SetTime)]
    public async Task<IActionResult> SetTime(string id, SetExamDto setExamDto)
    {
        var res = await finalService.SetExamDateAsync(setExamDto);
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean")]
    [HttpPut(ApiEndPoints.Finals.Update)]
    public async Task<IActionResult> Update(string id, UpdateExamDto updateExamDto)
    {
        var res = await finalService.UpdateAsync(new UpdateExamRequest(id, updateExamDto));
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Teacher,Dean")]
    [HttpPut(ApiEndPoints.Finals.Grade)]
    public async Task<IActionResult> GradeFinal([FromRoute] string id, [FromBody] int grade)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var res = await finalService.GradeAsync(userId, id, grade);
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean")]
    [HttpPut(ApiEndPoints.Finals.Confirm)]
    public async Task<IActionResult> ConfirmFinal([FromRoute] string id)
    {
        var res = await finalService.ConfirmAsync(id);
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }   
}