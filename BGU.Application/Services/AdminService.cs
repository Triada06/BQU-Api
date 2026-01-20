using BGU.Application.Common;
using BGU.Application.Dtos.Student;
using BGU.Application.Dtos.Teacher;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class AdminService(
    AppDbContext dbContext,
    UserManager<AppUser> userManager,
    IGroupRepository groupRepository,
    IStudentRepository studentRepository,
    IDepartmentRepository departmentRepository,
    ITeacherRepository teacherRepository) : IAdminService {
    public async Task<ApiResult<UserCreatedDto>> CreateStudentAsync(StudentDto dto) {
        var group = (
            await groupRepository.FindAsync(x => x.Code.Trim().ToLower() == dto.GroupName.Trim().ToLower(),
                include: x => x
                    .Include(g => g.Specialization)
                    .ThenInclude(g => g.Faculty)
                    .Include(g => g.AdmissionYear))
        ).FirstOrDefault();

        if (group == null) {
            return ApiResult<UserCreatedDto>.BadRequest(new UserCreatedDto {
                    FullName = BuildFullName(dto.Name, dto.Surname, dto.MiddleName),
                    UserName = dto.UserName
                }, $"Group with the name {dto.GroupName} not found");
        }

        var existingUser = await userManager.FindByNameAsync(dto.UserName);
        if (existingUser != null) {
            Console.WriteLine(existingUser.UserName);
            var existingStu = (await studentRepository.FindAsync(
                    x => x.AppUserId == existingUser.Id,
                    s => s.Include(m => m.AppUser)
                        .Include(m => m.StudentAcademicInfo)
                ))
                .SingleOrDefault();

            if (existingStu == null)
                return ApiResult<UserCreatedDto>.BadRequest(new UserCreatedDto {
                    FullName = BuildFullName(dto.Name, dto.Surname, dto.MiddleName),
                    UserName = dto.UserName
                }, "Student not found");

            existingStu.AppUser.Name = dto.Name;
            existingStu.AppUser.Surname = dto.Surname;
            existingStu.AppUser.MiddleName = dto.MiddleName;
            existingStu.AppUser.UserName = dto.UserName;
            existingStu.StudentAcademicInfo.FacultyId = group.Specialization.Faculty.Id;
            existingStu.StudentAcademicInfo.SpecializationId = group.Specialization.Id;
            existingStu.StudentAcademicInfo.GroupId = group.Id;
            existingStu.StudentAcademicInfo.AdmissionYearId = group.AdmissionYear.Id;
            existingStu.StudentAcademicInfo.AdmissionScore = dto.AdmissionScore;
            existingStu.StudentAcademicInfo.Gpa = 0.0;

            var res = await studentRepository.UpdateAsync(existingStu);
            if (res) {
                return ApiResult<UserCreatedDto>.Success(new UserCreatedDto {
                    FullName = BuildFullName(dto.Name, dto.Surname, dto.MiddleName),
                    UserName = dto.UserName
                });
            }

            return ApiResult<UserCreatedDto>.SystemError("Something went wrong while updating");
        }

        // Create user
        var user = new AppUser {
            Name = dto.Name,
            Surname = dto.Surname,
            MiddleName = dto.MiddleName,
            UserName = dto.UserName,
        };

        var tempPassword = GenerateTemporaryPassword("Student");
        var result = await userManager.CreateAsync(user, tempPassword);

        if (!result.Succeeded)
            return ApiResult<UserCreatedDto>.BadRequest(new UserCreatedDto {
                FullName = BuildFullName(dto.Name, dto.Surname, dto.MiddleName),
                UserName = dto.UserName
            }, string.Join(", ", result.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, "Student");

        // Create student
        var student = new Student {
            AppUserId = user.Id,
            StudentAcademicInfo = new StudentAcademicInfo {
                FacultyId = group.Specialization.Faculty.Id,
                SpecializationId = group.Specialization.Id,
                GroupId = group.Id,
                AdmissionYearId = group.AdmissionYear.Id,
                AdmissionScore = dto.AdmissionScore,
                Gpa = 0.0
            }
        };

        await dbContext.Students.AddAsync(student);
        await dbContext.SaveChangesAsync();

        return ApiResult<UserCreatedDto>.Success(new UserCreatedDto {
            TemporaryPassword = tempPassword,
            FullName = BuildFullName(user.Name, user.Surname, user.MiddleName),
            UserName = user.UserName
        });
    }

    public async Task<List<BulkImportResult>> BulkImportStudentsAsync(List<StudentDto> students) {
        var results = new List<BulkImportResult>();

        foreach (var studentDto in students) {
            try {
                var result = await CreateStudentAsync(studentDto);

                results.Add(new BulkImportResult {
                    Success = result.IsSucceeded,
                    Message = result.Message,
                    TemporaryPassword = result.IsSucceeded ? result.Data?.TemporaryPassword : null,
                    FullName = result.Data?.FullName,
                    UserName = result.Data?.UserName
                });
            }
            catch (Exception ex) {
                results.Add(new BulkImportResult {
                    Success = false,
                    Message = ex.Message,
                    FullName = BuildFullName(studentDto.Name, studentDto.Surname, studentDto.MiddleName),
                    UserName = studentDto.UserName
                });
            }
        }

        return results;
    }

    public async Task<ApiResult<UserCreatedDto>> CreateTeacherAsync(TeacherDto dto) {
        var department =
            (await departmentRepository.FindAsync(x => x.Name.Trim().ToLower() == dto.DepartmentName.Trim().ToLower()
            ))
            .FirstOrDefault();

        if (department == null) {
            return ApiResult<UserCreatedDto>.BadRequest(new UserCreatedDto {
                    FullName = BuildFullName(dto.Name, dto.Surname, dto.MiddleName),
                    UserName = dto.UserName
                }, $"Department with the name {dto.DepartmentName} not found");
        }

        var existingUser = await userManager.FindByNameAsync(dto.UserName);
        if (existingUser != null) {
            var existingTeacher = (await teacherRepository.FindAsync(
                    x => x.AppUserId == existingUser.Id,
                    t => t.Include(m => m.AppUser)
                ))
                .SingleOrDefault();

            if (existingTeacher == null)
                return ApiResult<UserCreatedDto>.BadRequest(new UserCreatedDto {
                    FullName = BuildFullName(dto.Name, dto.Surname, dto.MiddleName),
                    UserName = dto.UserName
                }, "Teacher not found");

            existingTeacher.AppUser.Name = dto.Name;
            existingTeacher.AppUser.Surname = dto.Surname;
            existingTeacher.AppUser.MiddleName = dto.MiddleName;
            existingTeacher.AppUser.UserName = dto.UserName;

            // NOTE: adjust property names if your Teacher entity differs.
            existingTeacher.TeacherAcademicInfo.DepartmentId = department.Id;
            existingTeacher.TeacherAcademicInfo.TeachingPosition = dto.Position;

            var res = await teacherRepository.UpdateAsync(existingTeacher);
            if (res) {
                return ApiResult<UserCreatedDto>.Success(new UserCreatedDto {
                    FullName = BuildFullName(dto.Name, dto.Surname, dto.MiddleName),
                    UserName = dto.UserName
                });
            }

            return ApiResult<UserCreatedDto>.SystemError("Something went wrong while updating");
        }

        // Create user
        var user = new AppUser {
            Name = dto.Name,
            Surname = dto.Surname,
            MiddleName = dto.MiddleName,
            UserName = dto.UserName,
        };

        var tempPassword = GenerateTemporaryPassword("Teacher");
        var result = await userManager.CreateAsync(user, tempPassword);

        if (!result.Succeeded)
            return ApiResult<UserCreatedDto>.BadRequest(new UserCreatedDto {
                FullName = BuildFullName(dto.Name, dto.Surname, dto.MiddleName),
                UserName = dto.UserName
            }, string.Join(", ", result.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, "Teacher");

        // Create teacher
        // NOTE: adjust property names if your Teacher entity differs.
        var teacher = new Teacher {
            AppUserId = user.Id,
            TeacherAcademicInfo = new TeacherAcademicInfo {
                DepartmentId = department.Id,
                TeachingPosition = dto.Position,
            }
        };

        await dbContext.Teachers.AddAsync(teacher);
        await dbContext.SaveChangesAsync();

        return ApiResult<UserCreatedDto>.Success(new UserCreatedDto {
            TemporaryPassword = tempPassword,
            FullName = BuildFullName(user.Name, user.Surname, user.MiddleName),
            UserName = user.UserName
        });
    }

    public async Task<List<BulkImportResult>> BulkImportTeachersAsync(List<TeacherDto> teachers) {
        var results = new List<BulkImportResult>();

        foreach (var teacherDto in teachers) {
            try {
                var result = await CreateTeacherAsync(teacherDto);

                results.Add(new BulkImportResult {
                    Success = result.IsSucceeded,
                    Message = result.Message,
                    TemporaryPassword = result.IsSucceeded ? result.Data?.TemporaryPassword : null,
                    FullName = result.Data?.FullName,
                    UserName = result.Data?.UserName
                });
            }
            catch (Exception ex) {
                results.Add(new BulkImportResult {
                    Success = false,
                    Message = ex.Message,
                    FullName = BuildFullName(teacherDto.Name, teacherDto.Surname, teacherDto.MiddleName),
                    UserName = teacherDto.UserName
                });
            }
        }

        return results;
    }

    private static string BuildFullName(string? name, string? surname, string? middleName) =>
        string.Join(" ", new[] { name, surname, middleName }.Where(x => !string.IsNullOrWhiteSpace(x)));

    private static string GenerateTemporaryPassword(string prefix) =>
        $"{prefix}{Guid.NewGuid().ToString("N").Substring(0, 8)}!";
}