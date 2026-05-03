using System.Security.Claims;
using BGU.Api.Helpers;
using BGU.Application.Contracts.Seminars.Requests;
using BGU.Application.Contracts.Student.Requests;
using BGU.Application.Contracts.User;
using BGU.Application.Services.Interfaces;
using BGU.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
public class StudentController(
    IStudentService studentService,
    IUserService userService,
    ITranscriptService transcriptService) : ControllerBase
{
    [Authorize(Roles = "Student")]
    [HttpGet(ApiEndPoints.Student.DashBoard)]
    public async Task<IActionResult> Dashboard()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var data = await studentService.Dashboard(userId);
        return Ok(data);
    }

    [Authorize(Roles = "Student")]
    [HttpGet(ApiEndPoints.Student.Schedule)]
    public async Task<IActionResult> Schedule(string schedule)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var data = await studentService.GetSchedule(userId, new StudentScheduleRequest(schedule));
        return Ok(data);
    }

    [Authorize(Roles = "Student")]
    [HttpGet(ApiEndPoints.Student.Grades)]
    public async Task<IActionResult> Grades([FromQuery] string grade = "courses")
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var data = await studentService.GetGrades(userId, new StudentGradesRequest(grade));
        return Ok(data);
    }

    [Authorize(Roles = "Student")]
    [HttpGet(ApiEndPoints.Student.Profile)]
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var data = await studentService.GetProfile(userId);
        return Ok(data);
    }

    [Authorize(Roles = "Dean")]
    [HttpGet(ApiEndPoints.Student.Filter)]
    public async Task<IActionResult> FilterBy([FromQuery] string? groupId, [FromQuery] int? year)
    {
        var res = await studentService.FilterAsync(groupId, year);
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean")]
    [HttpGet(ApiEndPoints.Student.Search)]
    public async Task<IActionResult> Search([FromQuery] string? searchText)
    {
        var res = await studentService.SearchAsync(searchText);
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean")]
    [HttpGet(ApiEndPoints.Student.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var res = await studentService.GetAllAsync(page, pageSize);
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean")]
    [HttpGet(ApiEndPoints.Student.GetById)]
    public async Task<IActionResult> GetById([FromRoute] string id, CancellationToken cancellationToken)
    {
        var res = await studentService.GetByIdAsync(id, cancellationToken);
        return res.IsSucceeded ? Ok(res) : StatusCode(res.StatusCode, res.Message);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut(ApiEndPoints.Student.MarkAbsence)]
    public async Task<IActionResult> MarkAbsence([FromRoute] string studentId, [FromRoute] string classId,
        [FromBody] MarkAbsenceRequest? markAbsenceRequest)
    {
        var res = await studentService.MarkAbsenceAsync(studentId, classId, markAbsenceRequest?.SeminarId);
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut(ApiEndPoints.Student.GradeColloquium)]
    public async Task<IActionResult> GradeColloquium([FromRoute] string studentId, [FromRoute] string colloquiumId,
        [FromQuery] Grade grade)
    {
        var res = await studentService.GradeStudentColloquiumAsync(
            new GradeStudentColloquiumRequest(studentId, colloquiumId, grade));
        return new ObjectResult(res);
    }


    [HttpGet(ApiEndPoints.Student.GetIndependentWorksByStudentId)]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetIndependentWorksByStudentId([FromRoute] string studentId,
        [FromRoute] string taughtSubjectId)
    {
        var res = await studentService.GetIndependentWorksByUserIdAsync(studentId, taughtSubjectId);
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Student")]
    [HttpGet(ApiEndPoints.Student.AcademicHistory)]
    public async Task<IActionResult> AcademicHistory(CancellationToken cp)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var res = await studentService.GetAcademicHistoryAsync(userId, cp);
        return res.IsSucceeded ? Ok(res) : StatusCode(res.StatusCode, res.Message);
    }

    [Authorize(Roles = "Student")]
    [HttpGet(ApiEndPoints.Student.GetFinals)]
    public async Task<IActionResult> GetFinals()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var res = await studentService.GetFinalsAsync(userId);

        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean")]
    [HttpDelete(ApiEndPoints.Student.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var res = await studentService.DeleteAsync(id);
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean")]
    [HttpPut(ApiEndPoints.Student.ResetPassword)]
    public async Task<IActionResult> ResetStudentPassword([FromRoute] string id,
        [FromBody] ChangePasswordDto changePasswordDto)
    {
        var response = await userService.ResetStudentPasswordAsync(id, changePasswordDto.NewPassword);
        Response.StatusCode = (int)response.StatusCode;
        return new ObjectResult(response);
    }

    [Authorize(Roles = "Student")]
    [HttpGet(ApiEndPoints.Student.GetTranscriptPdf)]
    public async Task<IActionResult> GenerateTranscriptPdf()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var res = await transcriptService.GeneratePdfAsync(userId);

        if (res.Data is null)
        {
            Response.StatusCode = res.StatusCode;
            return new ObjectResult(res);
        }

        return File(res.Data,
            "application/pdf",
            "transcript");
    }

    [Authorize(Roles = "Student")]
    [HttpGet(ApiEndPoints.Student.GetTranscriptExcel)]
    public async Task<IActionResult> GenerateTranscriptExcel()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var res = await transcriptService.GenerateExcelAsync(userId);


        if (res.Data is null)
        {
            Response.StatusCode = res.StatusCode;
            return new ObjectResult(res);
        }

        return File(res.Data,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "transcript");
    }
}