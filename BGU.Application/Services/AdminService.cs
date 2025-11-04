using BGU.Application.Common;
using BGU.Application.Dtos.Student;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class AdminService(AppDbContext dbContext, UserManager<AppUser> userManager) : IAdminService
{
    public async Task<ApiResult<StudentCreatedDto>> CreateStudentAsync(CreateStudentDto dto)
    {
        // Validate foreign keys
        var facultyExists = await dbContext.Faculties.AnyAsync(f => f.Id == dto.FacultyId);
        if (!facultyExists)
            return ApiResult<StudentCreatedDto>.BadRequest("Invalid faculty");
        
        var specExists = await dbContext.Specializations.AnyAsync(s => s.Id == dto.SpecializationId);
        if (!specExists)
            return ApiResult<StudentCreatedDto>.BadRequest("Invalid specialization");
        
        // Check if user already exists
        var existingUser = await userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return ApiResult<StudentCreatedDto>.BadRequest("Email already exists");
        
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
                FacultyId = dto.FacultyId,
                SpecializationId = dto.SpecializationId,
                GroupId = dto.GroupId,
                AdmissionYearId = dto.AdmissionYearId,
                EducationLanguage = dto.EducationLanguage,
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
            TemporaryPassword = tempPassword
        });
    }
    
    public async Task<List<BulkImportResult>> BulkImportStudentsAsync(List<CreateStudentDto> students)
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
                    TemporaryPassword = result.IsSucceeded ? result.Data?.TemporaryPassword : null
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
        
        return results;
    }
    
    private string GenerateTemporaryPassword()
    {
        return $"Student{Guid.NewGuid().ToString().Substring(0, 8)}!";
    }
}