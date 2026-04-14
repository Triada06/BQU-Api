using BGU.Api.Helpers;
using BGU.Application.Dtos.StudentEnrollment;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
public class StudentSubjectEnrollmentsController(IStudentSubjectEnrollmentService service) : ControllerBase
{
    [HttpPost(ApiEndPoints.StudentSubjectEnrollments.Create)]
    public async Task<IActionResult> Create(CreateStudentSubjectEnrollmentDto dto)
    {
        var result = await service.CreateAsync(dto);
        return Ok(result);
    }

    [HttpGet(ApiEndPoints.StudentSubjectEnrollments.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await service.GetAllAsync());
    }

    [HttpGet("{studentId}/{subjectId}/{attempt}")]
    public async Task<IActionResult> Get(string studentId, string subjectId, int attempt)
    {
        var result = await service.GetAsync(studentId, subjectId, attempt);
        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpPut("{studentId}/{subjectId}/{attempt}")]
    public async Task<IActionResult> Update(string studentId, string subjectId, int attempt, UpdateStudentSubjectEnrollmentDto dto)
    {
        var success = await service.UpdateAsync(studentId, subjectId, attempt, dto);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{studentId}/{subjectId}/{attempt}")]
    public async Task<IActionResult> Delete(string studentId, string subjectId, int attempt)
    {
        var success = await service.DeleteAsync(studentId, subjectId, attempt);
        if (!success) return NotFound();

        return NoContent();
    }
}