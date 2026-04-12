using BGU.Application.Dtos.StudentEnrollment;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentSubjectEnrollmentsController : ControllerBase
{
    private readonly IStudentSubjectEnrollmentService _service;

    public StudentSubjectEnrollmentsController(IStudentSubjectEnrollmentService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateStudentSubjectEnrollmentDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{studentId}/{subjectId}/{attempt}")]
    public async Task<IActionResult> Get(string studentId, string subjectId, int attempt)
    {
        var result = await _service.GetAsync(studentId, subjectId, attempt);
        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpPut("{studentId}/{subjectId}/{attempt}")]
    public async Task<IActionResult> Update(string studentId, string subjectId, int attempt, UpdateStudentSubjectEnrollmentDto dto)
    {
        var success = await _service.UpdateAsync(studentId, subjectId, attempt, dto);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{studentId}/{subjectId}/{attempt}")]
    public async Task<IActionResult> Delete(string studentId, string subjectId, int attempt)
    {
        var success = await _service.DeleteAsync(studentId, subjectId, attempt);
        if (!success) return NotFound();

        return NoContent();
    }
}