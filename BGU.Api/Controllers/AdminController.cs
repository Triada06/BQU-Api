using BGU.Api.Helpers;
using BGU.Application.Dtos.AppUser;
using BGU.Application.Dtos.Student;
using BGU.Application.Services.Interfaces;
using BGU.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
// [Authorize(Roles = "Admin,Dean,SuperAdmin")]
public class AdminController(
    IUserService userService,
    RoleManager<IdentityRole> roleManager,
    IExcelCrudService excelCrudService,
    IExcelService excelService,
    IAdminService adminService) : ControllerBase
{
    [HttpPost(ApiEndPoints.User.SignUp)]
    public async Task<IActionResult> SignUp([FromBody] AppUserSignUpDto request)
    {
        var res = await userService.SignUpAsync(request); //TODO: change to admin service
        return Ok(res);
    }

    [HttpGet("students/template")]
    public async Task<IActionResult> DownloadStudentTemplate()
    {
        var fileBytes = await excelService.GenerateStudentTemplateAsync();
        return File(fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "StudentImportTemplate.xlsx");
    }

    // Upload and import
    [HttpPost("students/import")]
    public async Task<IActionResult> ImportStudents(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (!file.FileName.EndsWith(".xlsx"))
            return BadRequest("Only .xlsx files are supported");

        try
        {
            using var stream = file.OpenReadStream();
            var students = await excelService.ParseStudentExcelAsync(stream);

            if (!students.Any())
                return BadRequest("No valid students found in file");

            // Import students
            var results = await adminService.BulkImportStudentsAsync(students);

            return Ok(new
            {
                totalProcessed = results.Count,
                successful = results.Count(r => r.Success),
                failed = results.Count(r => !r.Success),
                details = results
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing file: {ex.Message}");
        }
    }

    // Single student creation
    [HttpPost("students")]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto request)
    {
        var result = await adminService.CreateStudentAsync(request);
        return Ok(result);
    }


    [HttpGet("admission-years/template")]
    public async Task<IActionResult> DownloadAdmissionYearTemplate()
    {
        var fileBytes = await excelService.GenerateAdmissionYearTemplateAsync();
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "AdmissionYearTemplate.xlsx");
    }

    [HttpPost("admission-years/import")]
    public async Task<IActionResult> ImportAdmissionYears(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (!file.FileName.EndsWith(".xlsx"))
            return BadRequest("Only .xlsx files are supported");

        try
        {
            using var stream = file.OpenReadStream();
            var items = await excelService.ParseAdmissionYearExcelAsync(stream);
            var results = await excelCrudService.ProcessAdmissionYearsAsync(items);

            return Ok(new
            {
                totalProcessed = results.Count,
                successful = results.Count(r => r.Success),
                failed = results.Count(r => !r.Success),
                details = results
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing file: {ex.Message}");
        }
    }

    // ==================== FACULTY ====================

    [HttpGet("faculties/template")]
    public async Task<IActionResult> DownloadFacultyTemplate()
    {
        var fileBytes = await excelService.GenerateFacultyTemplateAsync();
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "FacultyTemplate.xlsx");
    }

    [HttpPost("faculties/import")]
    public async Task<IActionResult> ImportFaculties(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (!file.FileName.EndsWith(".xlsx"))
            return BadRequest("Only .xlsx files are supported");

        try
        {
            using var stream = file.OpenReadStream();
            var items = await excelService.ParseFacultyExcelAsync(stream);
            var results = await excelCrudService.ProcessFacultiesAsync(items);

            return Ok(new
            {
                totalProcessed = results.Count,
                successful = results.Count(r => r.Success),
                failed = results.Count(r => !r.Success),
                details = results
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing file: {ex.Message}");
        }
    }

    // ==================== DEPARTMENT ====================

    [HttpGet("departments/template")]
    public async Task<IActionResult> DownloadDepartmentTemplate()
    {
        var fileBytes = await excelService.GenerateDepartmentTemplateAsync();
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "DepartmentTemplate.xlsx");
    }

    [HttpPost("departments/import")]
    public async Task<IActionResult> ImportDepartments(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (!file.FileName.EndsWith(".xlsx"))
            return BadRequest("Only .xlsx files are supported");

        try
        {
            using var stream = file.OpenReadStream();
            var items = await excelService.ParseDepartmentExcelAsync(stream);
            var results = await excelCrudService.ProcessDepartmentsAsync(items);

            return Ok(new
            {
                totalProcessed = results.Count,
                successful = results.Count(r => r.Success),
                failed = results.Count(r => !r.Success),
                details = results
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing file: {ex.Message}");
        }
    }

    // ==================== SPECIALIZATION ====================

    [HttpGet("specializations/template")]
    public async Task<IActionResult> DownloadSpecializationTemplate()
    {
        var fileBytes = await excelService.GenerateSpecializationTemplateAsync();
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "SpecializationTemplate.xlsx");
    }

    [HttpPost("specializations/import")]
    public async Task<IActionResult> ImportSpecializations(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (!file.FileName.EndsWith(".xlsx"))
            return BadRequest("Only .xlsx files are supported");

        try
        {
            using var stream = file.OpenReadStream();
            var items = await excelService.ParseSpecializationExcelAsync(stream);
            var results = await excelCrudService.ProcessSpecializationsAsync(items);

            return Ok(new
            {
                totalProcessed = results.Count,
                successful = results.Count(r => r.Success),
                failed = results.Count(r => !r.Success),
                details = results
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing file: {ex.Message}");
        }
    }

    // ==================== GROUP ====================

    [HttpGet("groups/template")]
    public async Task<IActionResult> DownloadGroupTemplate()
    {
        var fileBytes = await excelService.GenerateGroupTemplateAsync();
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "GroupTemplate.xlsx");
    }

    [HttpPost("groups/import")]
    public async Task<IActionResult> ImportGroups(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (!file.FileName.EndsWith(".xlsx"))
            return BadRequest("Only .xlsx files are supported");

        try
        {
            using var stream = file.OpenReadStream();
            var items = await excelService.ParseGroupExcelAsync(stream);
            var results = await excelCrudService.ProcessGroupsAsync(items);

            return Ok(new
            {
                totalProcessed = results.Count,
                successful = results.Count(r => r.Success),
                failed = results.Count(r => !r.Success),
                details = results
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing file: {ex.Message}");
        }
    }

    // ==================== SUBJECT ====================

    [HttpGet("subjects/template")]
    public async Task<IActionResult> DownloadSubjectTemplate()
    {
        var fileBytes = await excelService.GenerateSubjectTemplateAsync();
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "SubjectTemplate.xlsx");
    }

    [HttpPost("subjects/import")]
    public async Task<IActionResult> ImportSubjects(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (!file.FileName.EndsWith(".xlsx"))
            return BadRequest("Only .xlsx files are supported");

        try
        {
            using var stream = file.OpenReadStream();
            var items = await excelService.ParseSubjectExcelAsync(stream);
            var results = await excelCrudService.ProcessSubjectsAsync(items);

            return Ok(new
            {
                totalProcessed = results.Count,
                successful = results.Count(r => r.Success),
                failed = results.Count(r => !r.Success),
                details = results
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing file: {ex.Message}");
        }
    }

    // ==================== LECTURE HALL ====================

    [HttpGet("lecture-halls/template")]
    public async Task<IActionResult> DownloadLectureHallTemplate()
    {
        var fileBytes = await excelService.GenerateLectureHallTemplateAsync();
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "LectureHallTemplate.xlsx");
    }

    [HttpPost("lecture-halls/import")]
    public async Task<IActionResult> ImportLectureHalls(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (!file.FileName.EndsWith(".xlsx"))
            return BadRequest("Only .xlsx files are supported");

        try
        {
            using var stream = file.OpenReadStream();
            var items = await excelService.ParseLectureHallExcelAsync(stream);
            var results = await excelCrudService.ProcessLectureHallsAsync(items);

            return Ok(new
            {
                totalProcessed = results.Count,
                successful = results.Count(r => r.Success),
                failed = results.Count(r => !r.Success),
                details = results
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing file: {ex.Message}");
        }
    }

    // ==================== CLASS TIME ====================

    [HttpGet("class-times/template")]
    public async Task<IActionResult> DownloadClassTimeTemplate()
    {
        var fileBytes = await excelService.GenerateClassTimeTemplateAsync();
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "ClassTimeTemplate.xlsx");
    }

    [HttpPost("class-times/import")]
    public async Task<IActionResult> ImportClassTimes(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (!file.FileName.EndsWith(".xlsx"))
            return BadRequest("Only .xlsx files are supported");

        try
        {
            using var stream = file.OpenReadStream();
            var items = await excelService.ParseClassTimeExcelAsync(stream);
            var results = await excelCrudService.ProcessClassTimesAsync(items);

            return Ok(new
            {
                totalProcessed = results.Count,
                successful = results.Count(r => r.Success),
                failed = results.Count(r => !r.Success),
                details = results
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing file: {ex.Message}");
        }
    }

    // ==================== TAUGHT SUBJECT ====================

    [HttpGet("taught-subjects/template")]
    public async Task<IActionResult> DownloadTaughtSubjectTemplate()
    {
        var fileBytes = await excelService.GenerateTaughtSubjectTemplateAsync();
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "TaughtSubjectTemplate.xlsx");
    }

    [HttpPost("taught-subjects/import")]
    public async Task<IActionResult> ImportTaughtSubjects(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (!file.FileName.EndsWith(".xlsx"))
            return BadRequest("Only .xlsx files are supported");

        try
        {
            using var stream = file.OpenReadStream();
            var items = await excelService.ParseTaughtSubjectExcelAsync(stream);
            var results = await excelCrudService.ProcessTaughtSubjectsAsync(items);

            return Ok(new
            {
                totalProcessed = results.Count,
                successful = results.Count(r => r.Success),
                failed = results.Count(r => !r.Success),
                details = results
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing file: {ex.Message}");
        }
    }

    // ==================== TEACHER ====================

    [HttpGet("teachers/template")]
    public async Task<IActionResult> DownloadTeacherTemplate()
    {
        var fileBytes = await excelService.GenerateTeacherTemplateAsync();
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "TeacherTemplate.xlsx");
    }

    [HttpPost("teachers/import")]
    public async Task<IActionResult> ImportTeachers(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (!file.FileName.EndsWith(".xlsx"))
            return BadRequest("Only .xlsx files are supported");

        try
        {
            using var stream = file.OpenReadStream();
            var items = await excelService.ParseTeacherExcelAsync(stream);
            var results = await excelCrudService.ProcessTeachersAsync(items);

            return Ok(new
            {
                totalProcessed = results.Count,
                successful = results.Count(r => r.Success),
                failed = results.Count(r => !r.Success),
                details = results
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing file: {ex.Message}");
        }
    }


    // [AllowAnonymous]
    // [HttpPost("api/addroles")]
    // public async Task<IActionResult> CreateRole()
    // {
    //     foreach (var role in Enum.GetValues<Roles>())
    //     {
    //         if (!await roleManager.RoleExistsAsync(role.ToString()))
    //             await roleManager.CreateAsync(new IdentityRole(role.ToString()));
    //     }
    //
    //     return Ok();
    // }
}