using BGU.Api.Helpers;
using BGU.Application.Dtos.Student;
using BGU.Application.Services;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Teacher,Dean")]
public class AdminController(
    IExcelService excelService,
    IAdminService adminService) : ControllerBase
{
    // Upload and import
    [HttpPost(ApiEndPoints.Student.Import)]
    public async Task<IActionResult> ImportStudents(IFormFile file)
    {
        if (file.Length == 0)
            return BadRequest("No file uploaded");

        if (!file.FileName.EndsWith(".xlsx"))
            return BadRequest("Only .xlsx files are supported");

        try
        {
            await using var stream = file.OpenReadStream();
            var students = await excelService.ParseStudentExcelAsync(stream);
            var results = await adminService.BulkImportStudentsAsync(students);
            var excelBytes = excelService.GenerateUserResultsExcel(results);

            var fileName = $"StudentImportResults_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing file: {ex.Message}");
        }
    }

    // Single student creation
    [HttpPost(ApiEndPoints.Student.Create)]
    public async Task<IActionResult> CreateStudent([FromBody] StudentDto request)
    {
        var result =
            await adminService
                .CreateStudentAsync(
                    request); //TODO: add independent work, colls, seminars, attendances creating on creating a student and adding to a group
        return Ok(result);
    }

    // ==================== TEACHER ====================

    [HttpPost(ApiEndPoints.Teacher.Import)]
    public async Task<IActionResult> ImportTeachers(IFormFile file)
    {
        if (file.Length == 0)
            return BadRequest("No file uploaded");

        if (!file.FileName.EndsWith(".xlsx"))
            return BadRequest("Only .xlsx files are supported");

        try
        {
            using var stream = file.OpenReadStream();
            var items = await excelService.ParseTeacherExcelAsync(stream);
            var results = await adminService.BulkImportTeachersAsync(items);
            var excelBytes = excelService.GenerateUserResultsExcel(results);

            var fileName = $"TeacherImportResults_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing file: {ex.Message}");
        }
    }
}