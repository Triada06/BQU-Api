using BGU.Application.Common;
using BGU.Application.Dtos.Student;
using BGU.Application.Dtos.Teacher;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Core.Enums;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class AdminService(
    AppDbContext dbContext,
    UserManager<AppUser> userManager,
    IGroupRepository groupRepository,
    IColloquiumRepository colloquiumRepository,
    IStudentRepository studentRepository,
    ISeminarRepository seminarRepository,
    IIndependentWorkRepository independentWorkRepository,
    IDepartmentRepository departmentRepository,
    IAttendanceRepository attendanceRepository,
    ITeacherRepository teacherRepository) : IAdminService
{
    public async Task<ApiResult<UserCreatedDto>> CreateStudentAsync(StudentDto dto)
    {
        var group = (
            await groupRepository.FindAsync(x => x.Code.Trim().ToLower() == dto.GroupName.Trim().ToLower(),
                include: x => x
                    .Include(g => g.Specialization)
                    .ThenInclude(g => g.Faculty)
                    .Include(g => g.AdmissionYear)
                    .Include(g => g.TaughtSubjects)
                    .ThenInclude(ts => ts.Classes)
                    .ThenInclude(c => c.ClassTime))
        ).FirstOrDefault();

        if (group == null)
        {
            return ApiResult<UserCreatedDto>.BadRequest(new UserCreatedDto
                {
                    FullName = BuildFullName(dto.Name, dto.Surname, dto.MiddleName),
                    UserName = dto.UserName
                }, $"Group with the name {dto.GroupName} not found");
        }

        var existingUser = await userManager.FindByNameAsync(dto.UserName);
        if (existingUser is not null)
        {
            return ApiResult<UserCreatedDto>.BadRequest(message: "User with this FIN code already exists");
        }

        // Create user
        var user = new AppUser
        {
            Name = dto.Name,
            Surname = dto.Surname,
            MiddleName = dto.MiddleName,
            UserName = dto.UserName,
        };

        var tempPassword = GenerateTemporaryPassword("Student");
        var result = await userManager.CreateAsync(user, tempPassword);

        if (!result.Succeeded)
            return ApiResult<UserCreatedDto>.BadRequest(new UserCreatedDto
            {
                FullName = BuildFullName(dto.Name, dto.Surname, dto.MiddleName),
                UserName = dto.UserName
            }, string.Join(", ", result.Errors.Select(e => e.Description)));

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
                AdmissionScore = dto.AdmissionScore,
                Gpa = 0.0
            }
        };
        if (!await studentRepository.CreateAsync(student))
        {
            return ApiResult<UserCreatedDto>.SystemError(message: "An error occured while creating the user");
        }

        //create required stuff
        foreach (var taughtSubject in group.TaughtSubjects)
        {
            if (!await CreateAcademicRequirementsAsync(taughtSubject.Classes.ToList(), student, taughtSubject.Id))
            {
                return ApiResult<UserCreatedDto>.SystemError("Failed to create academic info for student");
            }
        }


        return ApiResult<UserCreatedDto>.Success(new UserCreatedDto
        {
            TemporaryPassword = tempPassword,
            FullName = BuildFullName(user.Name, user.Surname, user.MiddleName),
            UserName = user.UserName
        });
    }

    public async Task<List<BulkImportResult>> BulkImportStudentsAsync(List<StudentDto> students)
    {
        var results = new List<BulkImportResult>();

        foreach (var studentDto in students)
        {
            try
            {
                var result = await CreateStudentAsync(studentDto);

                results.Add(new BulkImportResult
                {
                    Success = result.IsSucceeded,
                    Message = result.Message,
                    TemporaryPassword = result.IsSucceeded ? result.Data?.TemporaryPassword : null,
                    FullName = result.Data?.FullName,
                    UserName = result.Data?.UserName
                });
            }
            catch (Exception ex)
            {
                results.Add(new BulkImportResult
                {
                    Success = false,
                    Message = ex.Message,
                    FullName = BuildFullName(studentDto.Name, studentDto.Surname, studentDto.MiddleName),
                    UserName = studentDto.UserName
                });
            }
        }

        return results;
    }

    public async Task<ApiResult<UserCreatedDto>> CreateTeacherAsync(TeacherDto dto)
    {
        var department =
            (await departmentRepository.FindAsync(x => x.Name.Trim().ToLower() == dto.DepartmentName.Trim().ToLower()
            ))
            .FirstOrDefault();

        if (department == null)
        {
            return ApiResult<UserCreatedDto>.BadRequest(new UserCreatedDto
                {
                    FullName = BuildFullName(dto.Name, dto.Surname, dto.MiddleName),
                    UserName = dto.UserName
                }, $"Department with the name {dto.DepartmentName} not found");
        }

        var existingUser = await userManager.FindByNameAsync(dto.UserName);
        if (existingUser != null)
        {
            var existingTeacher = (await teacherRepository.FindAsync(
                    x => x.AppUserId == existingUser.Id,
                    t => t.Include(m => m.AppUser)
                ))
                .SingleOrDefault();

            if (existingTeacher == null)
                return ApiResult<UserCreatedDto>.BadRequest(new UserCreatedDto
                {
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
            if (res)
            {
                return ApiResult<UserCreatedDto>.Success(new UserCreatedDto
                {
                    FullName = BuildFullName(dto.Name, dto.Surname, dto.MiddleName),
                    UserName = dto.UserName
                });
            }

            return ApiResult<UserCreatedDto>.SystemError("Something went wrong while updating");
        }

        // Create user
        var user = new AppUser
        {
            Name = dto.Name,
            Surname = dto.Surname,
            MiddleName = dto.MiddleName,
            UserName = dto.UserName,
        };

        var tempPassword = GenerateTemporaryPassword("Teacher");
        var result = await userManager.CreateAsync(user, tempPassword);

        if (!result.Succeeded)
            return ApiResult<UserCreatedDto>.BadRequest(new UserCreatedDto
            {
                FullName = BuildFullName(dto.Name, dto.Surname, dto.MiddleName),
                UserName = dto.UserName
            }, string.Join(", ", result.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, "Teacher");

        // Create teacher
        // NOTE: adjust property names if your Teacher entity differs.
        var teacher = new Teacher
        {
            AppUserId = user.Id,
            TeacherAcademicInfo = new TeacherAcademicInfo
            {
                DepartmentId = department.Id,
                TeachingPosition = dto.Position,
            }
        };

        await dbContext.Teachers.AddAsync(teacher);
        await dbContext.SaveChangesAsync();

        return ApiResult<UserCreatedDto>.Success(new UserCreatedDto
        {
            TemporaryPassword = tempPassword,
            FullName = BuildFullName(user.Name, user.Surname, user.MiddleName),
            UserName = user.UserName
        });
    }

    public async Task<List<BulkImportResult>> BulkImportTeachersAsync(List<TeacherDto> teachers)
    {
        var results = new List<BulkImportResult>();

        foreach (var teacherDto in teachers)
        {
            try
            {
                var result = await CreateTeacherAsync(teacherDto);

                results.Add(new BulkImportResult
                {
                    Success = result.IsSucceeded,
                    Message = result.Message,
                    TemporaryPassword = result.IsSucceeded ? result.Data?.TemporaryPassword : null,
                    FullName = result.Data?.FullName,
                    UserName = result.Data?.UserName
                });
            }
            catch (Exception ex)
            {
                results.Add(new BulkImportResult
                {
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

    private async Task<bool> CreateAcademicRequirementsAsync(List<Class> classes, Student student,
        string taughtSubjectId)
    {
        var seminarTypes = classes.FindAll(x => x.ClassType == ClassType.Семинар);

        var attendances = new List<Attendance>();
        var seminars = new List<Seminar>();
        var independentWorks = new List<IndependentWork>();
        var colloquiums = new List<Colloquiums>();

        foreach (var seminarType in seminarTypes)
        {
            var seminar = new Seminar
            {
                StudentId = student.Id,
                TaughtSubjectId = seminarType.TaughtSubjectId,
                GotAt = seminarType.ClassTime.ClassDate.UtcDateTime,
                Grade = Grade.None
            };
            seminars.Add(seminar);
        }

        if (!await seminarRepository.BulkCreate(seminars))
        {
            return false;
        }

        // For EACH class, create attendance 
        foreach (var classItem in classes)
        {
            var att = new Attendance
                { StudentId = student!.Id, ClassId = classItem.Id, IsAbsent = false };
            attendances.Add(att);
        }

        if (!await attendanceRepository.BulkCreateAsync(attendances))
        {
            return false;
        }

        //create independent works

        for (int i = 0; i < 10; i++)
        {
            var independentWork = new IndependentWork
            {
                Number = i + 1,
                StudentId = student!.Id,
                TaughtSubjectId = taughtSubjectId,
                IsPassed = null
            };
            independentWorks.Add(independentWork);
        }

        if (!await independentWorkRepository.BulkCreateAsync(independentWorks))
        {
            return false;
        }

        for (int i = 0; i < 3; i++)
        {
            var coll = new Colloquiums
            {
                Grade = Grade.None,
                StudentId = student!.Id,
                TaughtSubjectId = taughtSubjectId,
            };
            colloquiums.Add(coll);
        }

        if (!await colloquiumRepository.BulkCreateAsync(colloquiums))
        {
            return false;
        }

        return true;
    }
}