using BGU.Application.Contracts.Student;
using BGU.Application.Contracts.Student.Requests;
using BGU.Application.Contracts.Student.Responses;
using BGU.Application.Dtos.AcademicPerformance;
using BGU.Application.Dtos.Class;
using BGU.Application.Dtos.Student;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Core.Enums;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class StudentService(
    UserManager<AppUser> userManager,
    IStudentRepository studentRepository,
    IUserRepository userRepository,
    ITaughtSubjectRepository taughtSubjectRepository) : IStudentService
{
    public async Task<StudentDashboardResponse> Dashboard(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return new StudentDashboardResponse(null,
                ResponseMessages.Unauthorized, false,
                (int)StatusCode.Unauthorized);
        }

        var student = (await studentRepository.FindAsync(
            s => s.AppUserId == userId,
            s => s.Include(st => st.StudentAcademicInfo)
                .ThenInclude(ai => ai.Group)
                .ThenInclude(g => g.TaughtSubjects)
                .ThenInclude(ts => ts.Subject)
                .Include(st => st.StudentAcademicInfo.Group.TaughtSubjects)
                .ThenInclude(ts => ts.Teacher)
                .ThenInclude(t => t.AppUser)
                .Include(st => st.StudentAcademicInfo.Group.TaughtSubjects)
                .ThenInclude(ts => ts.Classes)
                .ThenInclude(c => c.ClassTime)
        )).FirstOrDefault();
        if (student == null)
        {
            return new StudentDashboardResponse(
                null,
                ResponseMessages.NotFound,
                false,
                (int)StatusCode.NotFound
            );
        }

        int today = GetToday();
        var classesToday = student.StudentAcademicInfo.Group.TaughtSubjects
            .SelectMany(gs => gs.Classes)
            .Where(c => c.ClassTime.DaysOfTheWeek == (DaysOfTheWeek)today)
            .Select(c => new TodaysClassesDto(
                c.Id,
                c.TaughtSubject.Subject.Name,
                c.ClassType.ToString(),
                c.TaughtSubject.Teacher.AppUser.Name,
                new DateTimeOffset(DateTime.Today.Add(c.ClassTime.Start))
            ))
            .OrderBy(c => c.Period)
            .ToList();
        return new StudentDashboardResponse(new StudentDashboardDto(user.Name, classesToday), "Found", true, 200);
    }

    public async Task<StudentScheduleResponse> GetSchedule(string userId, StudentScheduleRequest request)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return new StudentScheduleResponse(null,
                ResponseMessages.Unauthorized, false,
                (int)StatusCode.Unauthorized);
        }

        var student = (await studentRepository.FindAsync(
            s => s.AppUserId == userId,
            s => s.Include(st => st.StudentAcademicInfo)
                .ThenInclude(ai => ai.Group)
                .ThenInclude(g => g.TaughtSubjects)
                .ThenInclude(ts => ts.Subject)
                .Include(st => st.StudentAcademicInfo.Group.TaughtSubjects)
                .ThenInclude(ts => ts.Teacher)
                .ThenInclude(t => t.AppUser)
                .Include(st => st.StudentAcademicInfo.Group.TaughtSubjects)
                .ThenInclude(ts => ts.Classes)
                .ThenInclude(c => c.ClassTime)
        )).FirstOrDefault();

        if (student == null)
        {
            return new StudentScheduleResponse(
                null,
                ResponseMessages.NotFound,
                false,
                (int)StatusCode.NotFound
            );
        }

        if (request.Schedule == "week")
        {
            var todayDate = DateTime.Today;

            int diff = (7 + (todayDate.DayOfWeek - DayOfWeek.Monday)) % 7;
            var weekStart = todayDate.AddDays(-diff);
            var weekEnd = weekStart.AddDays(4);

            var classesThisWeek = student.StudentAcademicInfo.Group.TaughtSubjects
                .SelectMany(gs => gs.Classes)
                .Where(c =>
                {
                    // Map DaysOfTheWeek (1–5) to actual date
                    var classDate = weekStart.AddDays(((int)c.ClassTime.DaysOfTheWeek) - 1);

                    return classDate >= weekStart && classDate <= weekEnd;
                })
                .Select(c =>
                {
                    var classDate = weekStart.AddDays(((int)c.ClassTime.DaysOfTheWeek) - 1);
                    var classDateTime = classDate.Add(c.ClassTime.Start);

                    return new TodaysClassesDto(
                        c.Id,
                        c.TaughtSubject.Subject.Name,
                        c.ClassType.ToString(),
                        c.TaughtSubject.Teacher.AppUser.Name,
                        new DateTimeOffset(classDateTime)
                    );
                })
                .OrderBy(c => c.Period)
                .ToList();
            return new StudentScheduleResponse(
                new StudentScheduleDto(DateTime.Now.ToString("dddd, MMM dd"), classesThisWeek),
                "Found", true, 200);
        }

        int today = GetToday();
        var classesToday = student.StudentAcademicInfo.Group.TaughtSubjects
            .SelectMany(gs => gs.Classes)
            .Where(c => c.ClassTime.DaysOfTheWeek == (DaysOfTheWeek)today)
            .Select(c => new TodaysClassesDto(
                c.Id,
                c.TaughtSubject.Subject.Name,
                c.ClassType.ToString(),
                c.TaughtSubject.Teacher.AppUser.Name,
                new DateTimeOffset(DateTime.Today.Add(c.ClassTime.Start))
            ))
            .OrderBy(c => c.Period)
            .ToList();
        return new StudentScheduleResponse(new StudentScheduleDto(DateTime.Now.ToString("dddd, MMM dd"), classesToday),
            "Found", true, 200);
    }

    public async Task<StudentGradesResponse> GetGrades(string userId, StudentGradesRequest request)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return new StudentGradesResponse(null, null,
                ResponseMessages.Unauthorized, false,
                (int)StatusCode.Unauthorized);
        }

        var student = (await studentRepository.FindAsync(
            s => s.AppUserId == userId,
            s => s.Include(st => st.StudentAcademicInfo)
                .ThenInclude(ai => ai.Group)
                .ThenInclude(g => g.TaughtSubjects)
                .ThenInclude(ts => ts.Subject)
                .Include(st => st.StudentAcademicInfo.Group.TaughtSubjects)
                .ThenInclude(ts => ts.Teacher)
                .ThenInclude(t => t.AppUser)
                .Include(st => st.StudentAcademicInfo.Group.TaughtSubjects)
                .ThenInclude(ts => ts.Classes)
                .ThenInclude(c => c.ClassTime)
        )).FirstOrDefault();

        if (student == null)
        {
            return new StudentGradesResponse(
                null,
                null,
                ResponseMessages.NotFound,
                false,
                (int)StatusCode.NotFound
            );
        }

        if (request.Grade == "sessions")
        {
            var sessions = student.StudentAcademicInfo.Group.TaughtSubjects
                .Select(c => new ClassSessions(
                    c.Subject.Name,
                    c.ClassSessions.Select(e => new ClassInfo(e.Date,
                        c.Classes.Where(x => x.CreatedAt == e.Date).Select(m => m.ClassType).First(),
                        e.Attendances.Where(x => x.CreatedAt == e.Date).Select(m => m.IsAbsent).First(),
                        student.SeminarGrades.Where(ex => ex.GotAt == e.Date).Select(m => (int)m.Grade).First())
                    ))
                )
                .ToList();
            return new StudentGradesResponse(null, sessions,
                "Ok", true, 200);
        }

        var taughtSubjects = student.StudentAcademicInfo.Group.TaughtSubjects;

        var classes = taughtSubjects
            .Select(c => new AcademicPerformanceDto(
                c.Subject.Name,
                c.Group.Code,
                c.Teacher.AppUser.Name,
                c.Subject.CreditsNumber,
                c.Hours,
                CalculateOverallSubjectScore(
                    c.Seminars.Where(x => x.StudentId == student.Id).Select(s => (int)s.Grade)
                        .ToList(),
                    c.Colloquiums.Where(x => x.StudentId == student.Id).Select(s => (int)s.Grade)
                        .ToList(),
                    (Grade)c.IndependentWorks.Where(x => x.StudentId == student.Id)
                        .Count(s => s.IsPassed)),
                c.Seminars.Where(x => x.StudentId == student.Id).Select(s => (int)s.Grade)
                    .ToList(),
                c.Colloquiums.Where(x => x.StudentId == student.Id).Select(s => (int)s.Grade)
                    .ToList(),
                c.IndependentWorks.Where(x => x.StudentId == student.Id)
                    .Count(s => s.IsPassed),
                c.ClassSessions
                    .Select(m => m.Attendances.Where(x => x.StudentId == student.Id).Select(x => x.IsAbsent)).Count(),
                c.Hours / 2)
            )
            .ToList();
        return new StudentGradesResponse(new StudentGradesDto(classes), null,
            "Ok", true, 200);
    }

    public async Task<StudentProfileResponse> GetProfile(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return new StudentProfileResponse(null, null,
                ResponseMessages.Unauthorized, false,
                (int)StatusCode.Unauthorized);
        }

        var student = (await studentRepository.FindAsync(
            s => s.AppUserId == userId,
            s => s.Include(st => st.StudentAcademicInfo)
                .ThenInclude(ai => ai.Group)
                .ThenInclude(g => g.TaughtSubjects)
                .ThenInclude(ts => ts.Subject)
                .Include(st => st.StudentAcademicInfo.AdmissionYear)
                .Include(st => st.StudentAcademicInfo.Faculty)
                .Include(st => st.StudentAcademicInfo.Specialization)
                .Include(st => st.StudentAcademicInfo.Group.TaughtSubjects)
                .ThenInclude(ts => ts.Teacher)
                .ThenInclude(t => t.AppUser)
                .Include(st => st.StudentAcademicInfo.Group.TaughtSubjects)
                .ThenInclude(ts => ts.Classes)
                .ThenInclude(c => c.ClassTime)
        )).FirstOrDefault();

        if (student == null)
        {
            return new StudentProfileResponse(
                null,
                null,
                ResponseMessages.NotFound,
                false,
                (int)StatusCode.NotFound
            );
        }

        var studentAcademicInfo = new StudentAcademicInfoDto(user.Name, user.Surname, student.Id,
            student.StudentAcademicInfo.Gpa, nameof(student.StudentAcademicInfo.Group.EducationLevel),
            student.StudentAcademicInfo.AdmissionYear.FirstYear, student.StudentAcademicInfo.Faculty.Name,
            student.StudentAcademicInfo.Specialization.Name);

        var studentProfileInfo = new StudentPersonalInfo(user.Email!, user.BornDate);
        return new StudentProfileResponse(studentAcademicInfo, studentProfileInfo, ResponseMessages.Success, true,
            (int)StatusCode.Ok);
    }

    public async Task<GetStudentResponse> FilterAsync(string groupId, int year)
    {
        throw new NotImplementedException();
    }

    public async Task<GetStudentResponse> SearchAsync(string searchString)
    {
        var users = await userManager.GetUsersInRoleAsync("Student");
        if (users.Count is 0)
        {
            return new GetStudentResponse(null, StatusCode.NotFound, false, ResponseMessages.NotFound);
        }

        var filteredUsers = users.Where(u =>
            u.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase) ||
            u.Surname.Contains(searchString, StringComparison.CurrentCultureIgnoreCase) ||
            u.MiddleName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase));

        List<Student> students = [];

        foreach (var user in filteredUsers)
        {
            var student = await studentRepository.GetByIdAsync(user.Id, x => x
                .Include(st => st.StudentAcademicInfo)
                .ThenInclude(st => st.Group)
                .Include(st => st.AppUser), false);

            if (student is not null)
            {
                students.Add(student);
            }
        }

        return new GetStudentResponse(students.Select(x => new GetStudentDto(x.Id,
                x.AppUser.Name + "  " + x.AppUser.Surname, x.StudentAcademicInfo.Group.Code,
                GetYear(x.StudentAcademicInfo.Group.CreatedAt, DateTime.Now))), StatusCode.Ok, true,
            ResponseMessages.Success);
    }

    public async Task<GetStudentResponse> GetAllAsync(int page, int pageSize)
    {
        var students =
            (await studentRepository.GetAllAsync(page, pageSize, false,
                x => x
                    .Include(st => st.StudentAcademicInfo)
                    .ThenInclude(st => st.Group)
                    .Include(st => st.AppUser)))
            .Select(x =>
                new GetStudentDto(x.Id, x.AppUser.Name + "  " + x.AppUser.Surname, x.StudentAcademicInfo.Group.Code,
                    GetYear(x.StudentAcademicInfo.Group.CreatedAt, DateTime.Now)));
        return new GetStudentResponse(students, StatusCode.Ok, true, ResponseMessages.Success);
    }

    private static double CalculateOverallSubjectScore(List<int> seminarScores, List<int> colloquiumScores,
        Grade assigment)
        => (colloquiumScores.Sum() / 0.4) + seminarScores.Sum() / 0.6 + (int)assigment;

    private static int GetToday()
        => (int)DateTime.Today.DayOfWeek;

    private static int GetYear(DateTime start, DateTime end)
    {
        var totalDays = (end - start).TotalDays;
        return (int)(totalDays / 365.2425);
    }
}