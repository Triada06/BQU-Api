using BGU.Api.Helpers;
using BGU.Application.Contracts.Syllabus.Requests;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Teacher")]
public class SyllabusController(ISyllabusService syllabusService) : ControllerBase
{
    [HttpPost(ApiEndPoints.Syllabus.Create)]
    public async Task<IActionResult> Create(IFormFile file, [FromRoute] string taughtSubjectId)
    {
        var res = await syllabusService.CreateAsync(new CreateSyllabusRequest(file, taughtSubjectId));
        return new ObjectResult(res);
    }

    [HttpPut(ApiEndPoints.Syllabus.Update)]
    public async Task<IActionResult> Update(IFormFile file, [FromRoute] string id)
    {
        var res = await syllabusService.UpdateAsync(new UpdateSyllabusRequest(file, id));
        return new ObjectResult(res);
    }

    [HttpDelete(ApiEndPoints.Syllabus.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var res = await syllabusService.DeleteAsync(id);
        return new ObjectResult(res);
    }

    [HttpGet(ApiEndPoints.Syllabus.GetById)]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var res = await syllabusService.GetByIdAsync(id);

        if (!res.IsSucceeded || res.Dto is null)
            return new ObjectResult(res);

        return File(
            res.Dto.Bytes,
            "application/pdf",
            res.Dto.FileName
        );
    }
}