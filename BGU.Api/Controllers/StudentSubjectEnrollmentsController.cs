using BGU.Api.Helpers;
using BGU.Application.Dtos.StudentEnrollment;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Dean")]
public class StudentSubjectEnrollmentsController(IStudentSubjectEnrollmentService service) : ControllerBase
{
    [HttpPost(ApiEndPoints.StudentSubjectEnrollments.Create)]
    public async Task<IActionResult> Create(CreateStudentSubjectEnrollmentDto dto)
    {
        var result = await service.CreateAsync(dto);
        Response.StatusCode = result.StatusCode;
        return new ObjectResult(result);
    }

    [HttpGet(ApiEndPoints.StudentSubjectEnrollments.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await service.GetAllAsync(page, pageSize);
        Response.StatusCode = result.StatusCode;
        return new ObjectResult(result);
    }

    [HttpGet(ApiEndPoints.StudentSubjectEnrollments.GetById)]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var result = await service.GetByIdAsync(id);
        Response.StatusCode = result.StatusCode;
        return new ObjectResult(result);
    }

    [HttpPut(ApiEndPoints.StudentSubjectEnrollments.Update)]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateStudentSubjectEnrollmentDto dto)
    {
        var result = await service.UpdateAsync(id, dto);
        Response.StatusCode = result.StatusCode;
        return new ObjectResult(result);
    }

    [HttpDelete(ApiEndPoints.StudentSubjectEnrollments.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var result = await service.DeleteAsync(id);
        Response.StatusCode = result.StatusCode;
        return new ObjectResult(result);
    }
}