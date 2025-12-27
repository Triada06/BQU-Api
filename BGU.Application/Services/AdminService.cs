using BGU.Application.Common;
using BGU.Application.Dtos.Student;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BGU.Application.Services;

public class AdminService(
    AppDbContext dbContext,
    UserManager<AppUser> userManager,
    IGroupRepository groupRepository,
    IStudentRepository studentRepository) : IAdminService
{
    public async Task<ApiResult<StudentCreatedDto>> CreateStudentAsync(CreateStudentDto dto)
    {
        var group =
            (await groupRepository.FindAsync(x => x.Code.Trim().ToLower() == dto.GroupName.Trim().ToLower(),
                include: x => x
                    .Include(g => g.Specialization)
                    .ThenInclude(g => g.Faculty)
                    .Include(g => g.AdmissionYear)))
            .FirstOrDefault();

        if (group == null)
        {
            return ApiResult<StudentCreatedDto>.BadRequest($"Group with the name {dto.GroupName} not found");
        }

        //check if FIN is unique
        var finExists = await dbContext.Users.AnyAsync(u => u.Pin == dto.PinCode);
        if (finExists)
        {
            return ApiResult<StudentCreatedDto>.BadRequest("Fin code already in use");
        }

        //checking admission year
        if (string.IsNullOrWhiteSpace(group.AdmissionYearId))
        {
            return ApiResult<StudentCreatedDto>.BadRequest(
                $"Group with the code name {group.Code} has no admission year");
        }

        // Check if user already exists
        var existingUser = await userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            // return ApiResult<StudentCreatedDto>.BadRequest("Email already exists");
        {
            var existingStus =
                await studentRepository.FindAsync(x => x.AppUserId == existingUser.Id, s => s.Include(m => m.AppUser));
            if (existingStus.Count == 0)
                return ApiResult<StudentCreatedDto>.BadRequest("Student not found");
            var existingStu = existingStus.First();
            if (existingStu is null)
            {
                return ApiResult<StudentCreatedDto>.BadRequest("Student not found");
            }

            existingStu.AppUser.Email = dto.Email;
            existingStu.AppUser.Name = dto.Name;
            existingStu.AppUser.Surname = dto.Surname;
            existingStu.AppUser.MiddleName = dto.MiddleName;
            existingStu.AppUser.Pin = dto.PinCode;
            existingStu.AppUser.BornDate = dto.BornDate;
            existingStu.AppUser.Gender = dto.Gender;
            if (existingStu.StudentAcademicInfo is not null)
            {
                existingStu.StudentAcademicInfo.FacultyId = group.Specialization.Faculty.Id;
                existingStu.StudentAcademicInfo.SpecializationId = group.Specialization.Id;
                existingStu.StudentAcademicInfo.GroupId = group.Code;
                existingStu.StudentAcademicInfo.AdmissionYearId = group.AdmissionYear.Id;
                existingStu.StudentAcademicInfo.EducationLanguage = group.EducationLanguage;
                existingStu.StudentAcademicInfo.FormOfEducation = dto.FormOfEducation;
                existingStu.StudentAcademicInfo.DecreeNumber = dto.DecreeNumber;
                existingStu.StudentAcademicInfo.AdmissionScore = dto.AdmissionScore;
                existingStu.StudentAcademicInfo.Gpa = 0.0;
            }

            var res = await studentRepository.UpdateAsync(existingStu);
            if (res)
            {
                return ApiResult<StudentCreatedDto>.Success(new StudentCreatedDto
                {
                    StudentId = existingUser.Id,
                    UserId = existingStu.AppUser.Id,
                    Email = existingStu.AppUser.Email,
                });
            }

            return ApiResult<StudentCreatedDto>.SystemError("Something went wrong while updating");
        }

        // Create user
        var user = new AppUser
        {
            UserName = dto.Email.Split('@')[0],
            Email = dto.Email,
            Name = dto.Name,
            Surname = dto.Surname,
            MiddleName = dto.MiddleName,
            Pin = dto.PinCode,
            Gender = dto.Gender,
            BornDate = dto.BornDate
        };

        var tempPassword = GenerateTemporaryPassword();
        var result = await userManager.CreateAsync(user, tempPassword);

        if (!result.Succeeded)
            return ApiResult<StudentCreatedDto>.BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, "Student");

        // Create student
        var student = new Student
        {
            AppUserId = user.Id,
            StudentAcademicInfo = new StudentAcademicInfo
            {
                FacultyId = group.Specialization.Faculty.Id,
                SpecializationId = group.Specialization.Id,
                GroupId = group.Id,
                AdmissionYearId = group.AdmissionYear.Id,
                EducationLanguage = group.EducationLanguage,
                FormOfEducation = dto.FormOfEducation,
                DecreeNumber = dto.DecreeNumber,
                AdmissionScore = dto.AdmissionScore,
                Gpa = 0.0
            }
        };

        await dbContext.Students.AddAsync(student);
        await dbContext.SaveChangesAsync();

        return ApiResult<StudentCreatedDto>.Success(new StudentCreatedDto
        {
            StudentId = student.Id,
            UserId = user.Id,
            Email = user.Email,
            TemporaryPassword = tempPassword,
            FullName = user.Name + user.Surname + user.MiddleName,
            FINCode = user.Pin
        });
    }

    public async Task<byte[]> BulkImportStudentsAsync(List<CreateStudentDto> students)
    {
        var results = new List<BulkImportResult>();

        foreach (var studentDto in students)
        {
            try
            {
                var result = await CreateStudentAsync(studentDto);

                results.Add(new BulkImportResult
                {
                    Email = studentDto.Email,
                    Success = result.IsSucceeded,
                    Message = result.Message,
                    TemporaryPassword = result.IsSucceeded ? result.Data?.TemporaryPassword : null,
                    FullName = result?.Data?.FullName,
                    FinCode = result?.Data?.FINCode
                });
            }
            catch (Exception ex)
            {
                results.Add(new BulkImportResult
                {
                    Email = studentDto.Email,
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        return GenerateStudentResultsExcel(results);
    }

    private byte[] GenerateStudentResultsExcel(List<BulkImportResult> results)
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Import Results");

        // Add summary section
        worksheet.Cells[1, 1].Value = "Student Import Summary";
        worksheet.Cells[1, 1].Style.Font.Bold = true;
        worksheet.Cells[1, 1].Style.Font.Size = 14;

        worksheet.Cells[2, 1].Value = "Total Processed:";
        worksheet.Cells[2, 2].Value = results.Count;

        worksheet.Cells[3, 1].Value = "Successful:";
        worksheet.Cells[3, 2].Value = results.Count(r => r.Success);
        worksheet.Cells[3, 2].Style.Font.Color.SetColor(System.Drawing.Color.Green);

        worksheet.Cells[4, 1].Value = "Failed:";
        worksheet.Cells[4, 2].Value = results.Count(r => !r.Success);
        worksheet.Cells[4, 2].Style.Font.Color.SetColor(System.Drawing.Color.Red);

        // Add headers for detailed results
        var headerRow = 6;
        worksheet.Cells[headerRow, 1].Value = "Email";
        worksheet.Cells[headerRow, 2].Value = "Status";
        worksheet.Cells[headerRow, 3].Value = "Message";
        worksheet.Cells[headerRow, 4].Value = "Temporary Password";
        worksheet.Cells[headerRow, 5].Value = "Name Surname";
        worksheet.Cells[headerRow, 6].Value = "FIN Code";

        // Style headers
        using (var range = worksheet.Cells[headerRow, 1, headerRow, 4])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
        }

        // Add data rows
        var currentRow = headerRow + 1;
        foreach (var result in results)
        {
            worksheet.Cells[currentRow, 1].Value = result.Email;
            worksheet.Cells[currentRow, 2].Value = result.Success ? "SUCCESS" : "FAILED";
            worksheet.Cells[currentRow, 3].Value = result.Message;
            worksheet.Cells[currentRow, 4].Value = result.TemporaryPassword ?? "";
            worksheet.Cells[currentRow, 5].Value = result.FullName ?? "";
            worksheet.Cells[currentRow, 6].Value = result.FinCode ?? "";

            // Color code status
            var statusCell = worksheet.Cells[currentRow, 2];
            if (result.Success)
            {
                statusCell.Style.Font.Color.SetColor(System.Drawing.Color.Green);
                statusCell.Style.Font.Bold = true;
            }
            else
            {
                statusCell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                statusCell.Style.Font.Bold = true;
            }

            currentRow++;
        }

        // Auto-fit columns
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        // Return as byte array
        return package.GetAsByteArray();
    }

    private string GenerateTemporaryPassword()
    {
        return $"Student{Guid.NewGuid().ToString().Substring(0, 8)}!";
    }

    private static AdmissionYear? ResolveAdmissionYear(string admissionYearString)
    {
        if (string.IsNullOrWhiteSpace(admissionYearString))
            return null;

        var digits = new string(admissionYearString.Where(char.IsDigit).ToArray());

        if (digits.Length != 8)
            return null;

        if (!int.TryParse(digits.Substring(0, 4), out int firstYear))
            return null;

        if (!int.TryParse(digits.Substring(4, 4), out int secondYear))
            return null;

        return new AdmissionYear
        {
            FirstYear = firstYear,
            SecondYear = secondYear
        };
    }
}