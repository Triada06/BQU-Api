using BGU.Api.Helpers;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Teacher")]
public class SyllabusController(ISyllabusService syllabusService) : ControllerBase
{
    [HttpPost(ApiEndPoints.Syllabus.Create)]
    public async Task<IActionResult> Create([FromForm] IFormFile file)
    {
        var res = await syllabusService.CreateAsync(new CreateSyllabusRequest(file));
        return new ObjectResult(res);
    }

    [HttpPost(ApiEndPoints.Syllabus.Update)]
    public async Task<IActionResult> Update([FromForm] IFormFile file, [FromRoute] string id)
    {
        var res = await syllabusService.UpdateAsync(new CreateSyllabusRequest(file, id));
        return new ObjectResult(res);
    }

    [HttpPost(ApiEndPoints.Syllabus.Delete)]
    public async Task<IActionResult> Update([FromRoute] string id)
    {
        var res = await syllabusService.DeleteAsync(id);
        return new ObjectResult(res);
    }

    [HttpPost(ApiEndPoints.Syllabus.GetById)]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var res = await syllabusService.GetByIdAsync(id);
        return new ObjectResult(res);
    }
}