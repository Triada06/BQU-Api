using BGU.Api.Helpers;
using BGU.Application.Dtos.AppUser;
using BGU.Application.Dtos.Student;
using BGU.Application.Services.Interfaces;
using BGU.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Teacher,Dean")]
public class AdminController(
    IUserService userService,
    RoleManager<IdentityRole> roleManager,
    IExcelCrudService excelCrudService,
    IExcelService excelService,
    IAdminService adminService) : ControllerBase
{
    [HttpGet(ApiEndPoints.Student.Template)]
    public async Task<IActionResult> DownloadStudentTemplate()
    {
        var fileBytes = await excelService.GenerateStudentTemplateAsync();
        return File(fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "StudentImportTemplate.xlsx");
    }

    // Upload and import
    [HttpPost(ApiEndPoints.Student.Import)]
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
            var excelBytes = await adminService.BulkImportStudentsAsync(students);

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
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto request)
    {
        var result = await adminService.CreateStudentAsync(request);
        return Ok(result);
    }

    //
    // [HttpGet(ApiEndPoints.AdmissionYear.Template)]
    // public async Task<IActionResult> DownloadAdmissionYearTemplate()
    // {
    //     var fileBytes = await excelService.GenerateAdmissionYearTemplateAsync();
    //     return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    //         "AdmissionYearTemplate.xlsx");
    // }
    //
    // [HttpPost(ApiEndPoints.AdmissionYear.Import)]
    // public async Task<IActionResult> ImportAdmissionYears(IFormFile file)
    // {
    //     if (file == null || file.Length == 0)
    //         return BadRequest("No file uploaded");
    //
    //     if (!file.FileName.EndsWith(".xlsx"))
    //         return BadRequest("Only .xlsx files are supported");
    //
    //     try
    //     {
    //         using var stream = file.OpenReadStream();
    //         var items = await excelService.ParseAdmissionYearExcelAsync(stream);
    //         var results = await excelCrudService.ProcessAdmissionYearsAsync(items);
    //
    //         return Ok(new
    //         {
    //             totalProcessed = results.Count,
    //             successful = results.Count(r => r.Success),
    //             failed = results.Count(r => !r.Success),
    //             details = results
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, $"Error processing file: {ex.Message}");
    //     }
    // }

    // ==================== FACULTY ====================
    //
    // [HttpGet(ApiEndPoints.Faculty.Template)]
    // public async Task<IActionResult> DownloadFacultyTemplate()
    // {
    //     var fileBytes = await excelService.GenerateFacultyTemplateAsync();
    //     return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    //         "FacultyTemplate.xlsx");
    // }
    //
    // [HttpPost(ApiEndPoints.Faculty.Template)]
    // public async Task<IActionResult> ImportFaculties(IFormFile file)
    // {
    //     if (file == null || file.Length == 0)
    //         return BadRequest("No file uploaded");
    //
    //     if (!file.FileName.EndsWith(".xlsx"))
    //         return BadRequest("Only .xlsx files are supported");
    //
    //     try
    //     {
    //         using var stream = file.OpenReadStream();
    //         var items = await excelService.ParseFacultyExcelAsync(stream);
    //         var results = await excelCrudService.ProcessFacultiesAsync(items);
    //
    //         return Ok(new
    //         {
    //             totalProcessed = results.Count,
    //             successful = results.Count(r => r.Success),
    //             failed = results.Count(r => !r.Success),
    //             details = results
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, $"Error processing file: {ex.Message}");
    //     }
    // }
    //
    // // ==================== DEPARTMENT ====================
    //
    // [HttpGet(ApiEndPoints.Department.Template)]
    // public async Task<IActionResult> DownloadDepartmentTemplate()
    // {
    //     var fileBytes = await excelService.GenerateDepartmentTemplateAsync();
    //     return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    //         "DepartmentTemplate.xlsx");
    // }
    //
    // [HttpPost(ApiEndPoints.Department.Import)]
    // public async Task<IActionResult> ImportDepartments(IFormFile file)
    // {
    //     if (file == null || file.Length == 0)
    //         return BadRequest("No file uploaded");
    //
    //     if (!file.FileName.EndsWith(".xlsx"))
    //         return BadRequest("Only .xlsx files are supported");
    //
    //     try
    //     {
    //         using var stream = file.OpenReadStream();
    //         var items = await excelService.ParseDepartmentExcelAsync(stream);
    //         var results = await excelCrudService.ProcessDepartmentsAsync(items);
    //
    //         return Ok(new
    //         {
    //             totalProcessed = results.Count,
    //             successful = results.Count(r => r.Success),
    //             failed = results.Count(r => !r.Success),
    //             details = results
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, $"Error processing file: {ex.Message}");
    //     }
    // }
    //
    // // ==================== SPECIALIZATION ====================
    //
    // [HttpGet(ApiEndPoints.Specialization.Template)]
    // public async Task<IActionResult> DownloadSpecializationTemplate()
    // {
    //     var fileBytes = await excelService.GenerateSpecializationTemplateAsync();
    //     return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    //         "SpecializationTemplate.xlsx");
    // }
    //
    // [HttpPost(ApiEndPoints.Specialization.Import)]
    // public async Task<IActionResult> ImportSpecializations(IFormFile file)
    // {
    //     if (file == null || file.Length == 0)
    //         return BadRequest("No file uploaded");
    //
    //     if (!file.FileName.EndsWith(".xlsx"))
    //         return BadRequest("Only .xlsx files are supported");
    //
    //     try
    //     {
    //         using var stream = file.OpenReadStream();
    //         var items = await excelService.ParseSpecializationExcelAsync(stream);
    //         var results = await excelCrudService.ProcessSpecializationsAsync(items);
    //
    //         return Ok(new
    //         {
    //             totalProcessed = results.Count,
    //             successful = results.Count(r => r.Success),
    //             failed = results.Count(r => !r.Success),
    //             details = results
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, $"Error processing file: {ex.Message}");
    //     }
    // }
    //
    // // ==================== GROUP ====================

    // [HttpGet(ApiEndPoints.Group.Template)]
    // public async Task<IActionResult> DownloadGroupTemplate()
    // {
    //     var fileBytes = await excelService.GenerateGroupTemplateAsync();
    //     return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    //         "GroupTemplate.xlsx");
    // }
    //
    // [HttpPost(ApiEndPoints.Group.Import)]
    // public async Task<IActionResult> ImportGroups(IFormFile file)
    // {
    //     if (file == null || file.Length == 0)
    //         return BadRequest("No file uploaded");
    //
    //     if (!file.FileName.EndsWith(".xlsx"))
    //         return BadRequest("Only .xlsx files are supported");
    //
    //     try
    //     {
    //         using var stream = file.OpenReadStream();
    //         var items = await excelService.ParseGroupExcelAsync(stream);
    //         var results = await excelCrudService.ProcessGroupsAsync(items);
    //
    //         return Ok(new
    //         {
    //             totalProcessed = results.Count,
    //             successful = results.Count(r => r.Success),
    //             failed = results.Count(r => !r.Success),
    //             details = results
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, $"Error processing file: {ex.Message}");
    //     }
    // }

    // ==================== SUBJECT ====================

    // [HttpGet(ApiEndPoints.Subject.Template)]
    // public async Task<IActionResult> DownloadSubjectTemplate()
    // {
    //     var fileBytes = await excelService.GenerateSubjectTemplateAsync();
    //     return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    //         "SubjectTemplate.xlsx");
    // }
    //
    // [HttpPost(ApiEndPoints.Subject.Import)]
    // public async Task<IActionResult> ImportSubjects(IFormFile file)
    // {
    //     if (file == null || file.Length == 0)
    //         return BadRequest("No file uploaded");
    //
    //     if (!file.FileName.EndsWith(".xlsx"))
    //         return BadRequest("Only .xlsx files are supported");
    //
    //     try
    //     {
    //         using var stream = file.OpenReadStream();
    //         var items = await excelService.ParseSubjectExcelAsync(stream);
    //         var results = await excelCrudService.ProcessSubjectsAsync(items);
    //
    //         return Ok(new
    //         {
    //             totalProcessed = results.Count,
    //             successful = results.Count(r => r.Success),
    //             failed = results.Count(r => !r.Success),
    //             details = results
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, $"Error processing file: {ex.Message}");
    //     }
    // }

    // ==================== LECTURE HALL ====================
    //
    // [HttpGet(ApiEndPoints.LectureHall.Template)]
    // public async Task<IActionResult> DownloadLectureHallTemplate()
    // {
    //     var fileBytes = await excelService.GenerateLectureHallTemplateAsync();
    //     return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    //         "LectureHallTemplate.xlsx");
    // }
    //
    // [HttpPost(ApiEndPoints.LectureHall.Import)]
    // public async Task<IActionResult> ImportLectureHalls(IFormFile file)
    // {
    //     if (file == null || file.Length == 0)
    //         return BadRequest("No file uploaded");
    //
    //     if (!file.FileName.EndsWith(".xlsx"))
    //         return BadRequest("Only .xlsx files are supported");
    //
    //     try
    //     {
    //         using var stream = file.OpenReadStream();
    //         var items = await excelService.ParseLectureHallExcelAsync(stream);
    //         var results = await excelCrudService.ProcessLectureHallsAsync(items);
    //
    //         return Ok(new
    //         {
    //             totalProcessed = results.Count,
    //             successful = results.Count(r => r.Success),
    //             failed = results.Count(r => !r.Success),
    //             details = results
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, $"Error processing file: {ex.Message}");
    //     }
    // }

    // ==================== CLASS TIME ====================

    // [HttpGet(ApiEndPoints.ClassTime.Template)]
    // public async Task<IActionResult> DownloadClassTimeTemplate()
    // {
    //     var fileBytes = await excelService.GenerateClassTimeTemplateAsync();
    //     return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    //         "ClassTimeTemplate.xlsx");
    // }
    //
    // [HttpPost(ApiEndPoints.ClassTime.Import)]
    // public async Task<IActionResult> ImportClassTimes(IFormFile file)
    // {
    //     if (file == null || file.Length == 0)
    //         return BadRequest("No file uploaded");
    //
    //     if (!file.FileName.EndsWith(".xlsx"))
    //         return BadRequest("Only .xlsx files are supported");
    //
    //     try
    //     {
    //         using var stream = file.OpenReadStream();
    //         var items = await excelService.ParseClassTimeExcelAsync(stream);
    //         var results = await excelCrudService.ProcessClassTimesAsync(items);
    //
    //         return Ok(new
    //         {
    //             totalProcessed = results.Count,
    //             successful = results.Count(r => r.Success),
    //             failed = results.Count(r => !r.Success),
    //             details = results
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, $"Error processing file: {ex.Message}");
    //     }
    // }

    // // ==================== TAUGHT SUBJECT ====================
    //
    // [HttpGet(ApiEndPoints.TaughtSubject.Template)]
    // public async Task<IActionResult> DownloadTaughtSubjectTemplate()
    // {
    //     var fileBytes = await excelService.GenerateTaughtSubjectTemplateAsync();
    //     return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    //         "TaughtSubjectTemplate.xlsx");
    // }
    //
    // [HttpPost(ApiEndPoints.TaughtSubject.Import)]
    // public async Task<IActionResult> ImportTaughtSubjects(IFormFile file)
    // {
    //     if (file == null || file.Length == 0)
    //         return BadRequest("No file uploaded");
    //
    //     if (!file.FileName.EndsWith(".xlsx"))
    //         return BadRequest("Only .xlsx files are supported");
    //
    //     try
    //     {
    //         using var stream = file.OpenReadStream();
    //         var items = await excelService.ParseTaughtSubjectExcelAsync(stream);
    //         var results = await excelCrudService.ProcessTaughtSubjectsAsync(items);
    //
    //         return Ok(new
    //         {
    //             totalProcessed = results.Count,
    //             successful = results.Count(r => r.Success),
    //             failed = results.Count(r => !r.Success),
    //             details = results
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, $"Error processing file: {ex.Message}");
    //     }
    // }

    // ==================== TEACHER ====================

    [HttpGet(ApiEndPoints.Teacher.Template)]
    public async Task<IActionResult> DownloadTeacherTemplate()
    {
        var fileBytes = await excelService.GenerateTeacherTemplateAsync();
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "TeacherTemplate.xlsx");
    }

    [HttpPost(ApiEndPoints.Teacher.Import)]
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
            var excelBytes = await excelCrudService.ProcessTeachersAsync(items);

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


    // [HttpGet("classes/template")]
    // public async Task<IActionResult> DownloadClassTemplate()
    // {
    //     var fileBytes = await excelService.GenerateClassTemplateAsync();
    //     return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
    //         "ClassTemplate.xlsx");
    // }
    //
    // [HttpPost("classes/import")]
    // public async Task<IActionResult> ImportClasses(IFormFile file)
    // {
    //     if (file == null || file.Length == 0)
    //         return BadRequest("No file uploaded");
    //
    //     if (!file.FileName.EndsWith(".xlsx"))
    //         return BadRequest("Only .xlsx files are supported");
    //
    //     try
    //     {
    //         using var stream = file.OpenReadStream();
    //         var items = await excelService.ParseClassExcelAsync(stream);
    //         var results = await excelCrudService.ProcessClassesAsync(items);
    //     
    //         return Ok(new
    //         {
    //             totalProcessed = results.Count,
    //             successful = results.Count(r => r.Success),
    //             failed = results.Count(r => !r.Success),
    //             details = results
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, $"Error processing file: {ex.Message}");
    //     }
    // }   
    //todo: FIX TIME SPANS FOR CLASSTIME

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