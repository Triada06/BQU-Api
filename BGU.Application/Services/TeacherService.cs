using BGU.Application.Contracts.Student.Responses;
using BGU.Application.Contracts.Teacher.Requests;
using BGU.Application.Contracts.Teacher.Responses;
using BGU.Application.Dtos.Class;
using BGU.Application.Dtos.Student;
using BGU.Application.Dtos.Teacher;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Core.Enums;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class TeacherService(UserManager<AppUser> userManager, ITeacherRepository teacherRepository) : ITeacherService {
    public async Task<TeacherProfileResponse> GetProfile(string userId) {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) {
            return new TeacherProfileResponse(null,
                ResponseMessages.Unauthorized, false,
                (int)StatusCode.Unauthorized);
        }

        var teacher = (await teacherRepository.FindAsync(
            s => s.AppUserId == userId,
            s => s.Include(st => st.TeacherAcademicInfo)
                .ThenInclude(ai => ai.Department)
                .ThenInclude(g => g.Faculty)
                .ThenInclude(ts => ts.Specializations)
        )).FirstOrDefault();

        if (teacher == null) {
            return new TeacherProfileResponse(
                null,
                ResponseMessages.NotFound,
                false,
                (int)StatusCode.NotFound
            );
        }

        var teacherSpecialization = teacher.TeacherAcademicInfo.Department.Faculty.Specializations
            .Where(t => t.FacultyId == teacher.TeacherAcademicInfo.Department.FacultyId).Select(x => x.Name)
            .FirstOrDefault();
        if (teacherSpecialization == null) {
            return new TeacherProfileResponse(
                null,
                "Teacher Specialization not found",
                false,
                (int)StatusCode.NotFound);
        }

        var teacherAcademicInfoDto = new TeacherAcademicInfoDto(user.Name, user.Surname, user.UserName, teacher.Id,
            teacher.TeacherAcademicInfo.Department.Faculty.Name, teacher.TeacherAcademicInfo.Department.Faculty.Name,
            teacherSpecialization);
        return new TeacherProfileResponse(teacherAcademicInfoDto, ResponseMessages.Success,
            true, (int)StatusCode.Ok);
    }

    public async Task<TeacherScheduleResponse> GetSchedule(TeacherScheduleRequest request) {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user is null) {
            return new TeacherScheduleResponse(null,
                ResponseMessages.Unauthorized, false,
                (int)StatusCode.Unauthorized);
        }

        var teacher = (await teacherRepository.FindAsync(
            s => s.AppUserId == request.UserId,
            s =>
                s.Include(x => x.TaughtSubjects)
                    .ThenInclude(x => x.Classes)
                    .Include(st => st.TeacherAcademicInfo)
                    .ThenInclude(ai => ai.Department)
                    .ThenInclude(g => g.Faculty)
                    .ThenInclude(ts => ts.Specializations)
        )).FirstOrDefault();

        if (teacher == null) {
            return new TeacherScheduleResponse(
                null,
                ResponseMessages.NotFound,
                false,
                (int)StatusCode.NotFound
            );
        }

        if (request.Schedule == "week") {
            var todayDate = DateTime.Today;

            int diff = (7 + (todayDate.DayOfWeek - DayOfWeek.Monday)) % 7;
            var weekStart = todayDate.AddDays(-diff);
            var weekEnd = weekStart.AddDays(4);
            var classesThisWeek = teacher.TaughtSubjects.SelectMany(x => x.Classes)
                .Where(c => {
                    // Map DaysOfTheWeek (1–5) to actual date
                    var classDate = weekStart.AddDays(((int)c.ClassTime.DaysOfTheWeek) - 1);

                    return classDate >= weekStart && classDate <= weekEnd;
                }).Select(c => {
                    var classDate = weekStart.AddDays(((int)c.ClassTime.DaysOfTheWeek) - 1);
                    var classDateTime = classDate.Add(c.ClassTime.Start);

                    return new TodaysClassesDto(
                        c.Id,
                        c.TaughtSubject.Subject.Name,
                        c.ClassType.ToString(),
                        c.TaughtSubject.Teacher.AppUser.Name,
                        new DateTimeOffset(classDateTime),
                        c.Room,
                        c.TaughtSubject.Code
                    );
                })
                .OrderBy(c => c.Period)
                .ToList();

            return new TeacherScheduleResponse(
                new TeacherScheduleDto(DateTime.Now.ToString("dddd, MMM dd"), classesThisWeek),
                "Found", true, 200);
        }

        int today = GetToday();
        var classesToday = teacher.TaughtSubjects.SelectMany(x => x.Classes)
            .Where(c => c.ClassTime.DaysOfTheWeek == (DaysOfTheWeek)today).Select(c => new TodaysClassesDto(
                c.Id,
                c.TaughtSubject.Subject.Name,
                c.ClassType.ToString(),
                c.TaughtSubject.Teacher.AppUser.Name,
                new DateTimeOffset(DateTime.Today.Add(c.ClassTime.Start)),
                c.Room, c.TaughtSubject.Code
            )).OrderBy(c => c.Period)
            .ToList();
        return new TeacherScheduleResponse(new TeacherScheduleDto(DateTime.Now.ToString("dddd, MMM dd"), classesToday),
            "Found", true, 200);
    }

    public async Task<TeacherCoursesResponse> GetCourses(string userId) {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) {
            return new TeacherCoursesResponse(null,
                ResponseMessages.Unauthorized, false,
                (int)StatusCode.Unauthorized);
        }

        var teacher = (await teacherRepository.FindAsync(
            s => s.AppUserId == userId,
            s =>
                s.Include(x => x.TaughtSubjects)
                    .ThenInclude(x => x.Group)
                    .ThenInclude(x => x.Students)
                    .Include(x => x.TaughtSubjects)
                    .ThenInclude(x => x.Classes)
                    .Include(st => st.TeacherAcademicInfo)
                    .ThenInclude(ai => ai.Department)
                    .ThenInclude(g => g.Faculty)
                    .ThenInclude(ts => ts.Specializations)
                    .Include(x => x.TaughtSubjects)
                    .ThenInclude(x => x.Subject)
        )).FirstOrDefault();

        if (teacher == null) {
            return new TeacherCoursesResponse(
                null,
                ResponseMessages.NotFound,
                false,
                (int)StatusCode.NotFound
            );
        }

        var courses =
            teacher.TaughtSubjects?.Select(x =>
                new TeacherCourseDto(x.Id, x.Subject.Name, x.Code, x.Group.Code, x.Subject.CreditsNumber,
                    x.Group.Students.Count, x.Hours));
        return new TeacherCoursesResponse(courses, ResponseMessages.Success, true, (int)StatusCode.Ok);
    }

    public async Task<UpdateTeacherResponse> UpdateAsync(string teacherId, UpdateTeacherRequest request) {
        var teacher = (await teacherRepository.FindAsync(x => x.Id == teacherId,
            i => i.Include(x => x.TeacherAcademicInfo).Include(x => x.AppUser), tracking: true)).FirstOrDefault();
        if (teacher is null)
            return new UpdateTeacherResponse(null, StatusCode.NotFound, false, ResponseMessages.NotFound);

        teacher.AppUser.Name = request.Name;
        teacher.AppUser.Surname = request.Surname;
        teacher.TeacherAcademicInfo.DepartmentId = request.DepartmentId;

        await teacherRepository.UpdateAsync(teacher);
        return new UpdateTeacherResponse(
            teacherId, StatusCode.Ok, false, ResponseMessages.Success);
    }

    public async Task<DeleteTeacherResponse> DeleteAsync(string teacherId) {
        var teacher = (await teacherRepository.FindAsync(x => x.Id == teacherId,
            i => i.Include(x => x.AppUser), tracking: true)).FirstOrDefault();
        if (teacher is null) {
            return new DeleteTeacherResponse(StatusCode.NotFound, false, ResponseMessages.NotFound);
        }

        if (!await teacherRepository.DeleteAsync(teacher)) {
            return new DeleteTeacherResponse(StatusCode.BadRequest, false,
                string.Join(", ", ResponseMessages.Failed));
        }

        var res = await userManager.DeleteAsync(teacher.AppUser);
        if (!res.Succeeded) {
            return new DeleteTeacherResponse(StatusCode.BadRequest, false,
                string.Join(", ", res.Errors.Select(x => x.Description)));
        }


        return new DeleteTeacherResponse(StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<GetByIdTeacherResponse> GetByIdAsync(string teacherId) {
        var teacher = await teacherRepository.GetByIdAsync(teacherId,
            i => i.Include(x => x.TeacherAcademicInfo)
                .Include(x => x.AppUser), tracking: false);
        if (teacher is null)
            return new GetByIdTeacherResponse(null, ResponseMessages.NotFound, false, StatusCode.NotFound);
        return new GetByIdTeacherResponse(
            new GetTeacherDto(teacher.Id,teacher.AppUser.Name, teacher.AppUser.Surname,
                teacher.AppUser.MiddleName, teacher.AppUser.UserName, teacher.AppUser.Gender,
                teacher.TeacherAcademicInfo.DepartmentId, teacher.TeacherAcademicInfo.TeachingPosition),
            ResponseMessages.Success,
            true, StatusCode.Ok);
    }

    public async Task<GetAllTeachersResponse> GetAllAsync(int page, int pageSize, bool tracking = false) {
        var teachers = await teacherRepository.GetAllAsync(page, pageSize,
            include: i => i.Include(x => x.TeacherAcademicInfo)
                .Include(x => x.AppUser),
            tracking: false);
        return new GetAllTeachersResponse(
            teachers.Select(x => new GetTeacherDto(x.Id,x.AppUser.Name,
                x.AppUser.Surname, x.AppUser.MiddleName,
                x.AppUser.UserName!, x.AppUser.Gender,
                x.TeacherAcademicInfo.DepartmentId, x.TeacherAcademicInfo.TeachingPosition)),
            ResponseMessages.Success,
            true, StatusCode.Ok);
    }


    private static int GetToday()
        => (int)DateTime.Today.DayOfWeek;
}