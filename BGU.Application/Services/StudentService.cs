using BGU.Application.Common;
using BGU.Application.Contracts.Student.Requests;
using BGU.Application.Contracts.Student.Responses;
using BGU.Application.Dtos.AcademicPerformance;
using BGU.Application.Dtos.Class;
using BGU.Application.Dtos.IndependentWorks;
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
    ISeminarRepository seminarRepository,
    IAttendanceService attendanceService,
    IColloquiumRepository colloquiumRepository,
    ITaughtSubjectRepository taughtSubjectRepository,
    IIndependentWorkRepository independentWorkRepository) : IStudentService
{
    private static readonly Dictionary<int, (int onePoint, int twoPoint, int forbidden)> AttendanceRules =
        new() // to calculate GPA
        {
            { 15, (1, 2, 3) },
            { 16, (1, 2, 3) },
            { 30, (2, 3, 4) },
            { 45, (3, 5, 6) },
            { 50, (3, 5, 6) },
            { 60, (3, 6, 8) },
            { 75, (4, 8, 10) },
            { 90, (5, 9, 12) },
            { 105, (6, 11, 14) }
        };


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
                new DateTimeOffset(DateTime.Today.Add(c.ClassTime.Start)),
                c.Room, c.TaughtSubject.Code
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
                        new DateTimeOffset(classDateTime),
                        c.Room, c.TaughtSubject.Code
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
                new DateTimeOffset(DateTime.Today.Add(c.ClassTime.Start)),
                c.Room, c.TaughtSubject.Code
            ))
            .OrderBy(c => c.Period)
            .ToList();
        return new StudentScheduleResponse(new StudentScheduleDto(DateTime.Now.ToString("dddd, MMM dd"), classesToday),
            "Found", true, 200);
    }


    //TODO: REFACTOR THIS METHOD    
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
                .Include(x => x.SeminarGrades)
                .Include(x => x.Attendances)
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
                    c.Classes.Select(e => new ClassInfo(e.ClassTime.ClassDate,
                        e.ClassType.ToString(),
                        student.Attendances.Where(x => x.ClassId == e.Id).Select(m => m.IsAbsent).First(),
                        e.ClassType == ClassType.Семинар
                            ? (int?)student.SeminarGrades.FirstOrDefault(x =>
                                x.TaughtSubjectId == c.Id && x.GotAt == e.ClassTime.ClassDate)?.Grade
                            : null)
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
                        .Count(s => s.IsPassed is true), c.Hours,
                    student.Attendances
                        .Count(attendance => c.Classes.Select(x => x.Id).Contains(attendance.ClassId))),
                c.Seminars.Where(x => x.StudentId == student.Id).Select(s => (int)s.Grade)
                    .ToList(),
                c.Colloquiums.Where(x => x.StudentId == student.Id).Select(s => (int)s.Grade)
                    .ToList(),
                c.IndependentWorks.Where(x => x.StudentId == student.Id)
                    .Count(s => s.IsPassed is true),
                student.Attendances
                    .Where(x => x.StudentId == student.Id).Select(x => x.IsAbsent).Count(),
                c.Classes.Count)
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
            return new StudentProfileResponse(null,
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
                ResponseMessages.NotFound,
                false,
                (int)StatusCode.NotFound
            );
        }

        var studentAcademicInfo = new StudentAcademicInfoDto(user.Name, user.Surname, user.UserName, student.Id,
            student.StudentAcademicInfo.Gpa, nameof(student.StudentAcademicInfo.Group.EducationLevel),
            student.StudentAcademicInfo.AdmissionYear.FirstYear, student.StudentAcademicInfo.Faculty.Name,
            student.StudentAcademicInfo.Specialization.Name);

        return new StudentProfileResponse(studentAcademicInfo, ResponseMessages.Success, true,
            (int)StatusCode.Ok);
    }

    public async Task<GetStudentResponse> FilterAsync(string? groupId, int? year)
    {
        var students = (await studentRepository.GetAllAsync(1, 10000, false, x => x
            .Include(st => st.StudentAcademicInfo)
            .ThenInclude(st => st.Group)
            .Include(st => st.StudentAcademicInfo.AdmissionYear)
            .Include(st => st.StudentAcademicInfo.Specialization)
            .Include(st => st.AppUser))).ToList();
        if (students.Count == 0)
        {
            return new GetStudentResponse([], StatusCode.NotFound, true, ResponseMessages.NotFound);
        }

        if (groupId is not null)
        {
            students = students.Where(x => x.StudentAcademicInfo.GroupId == groupId).ToList();
        }

        if (year is not null && students.Count is not 0)
        {
            students = students.Where(x => GetYear(x.StudentAcademicInfo.Group.CreatedAt, DateTime.Now) == year)
                .ToList();
        }

        return new GetStudentResponse(students.Count == 0
                ? []
                : students.Select(x => new GetStudentDto(
                    x.AppUser.Name + " " + x.AppUser.Surname + " " + x.AppUser.MiddleName, x.AppUser.UserName,
                    x.StudentAcademicInfo.Group.Code,
                    GetYear(x.StudentAcademicInfo.Group.CreatedAt, DateTime.Now),
                    x.StudentAcademicInfo.Specialization.Name,
                    x.StudentAcademicInfo.AdmissionYear.FirstYear + "/" +
                    x.StudentAcademicInfo.AdmissionYear.SecondYear, x.StudentAcademicInfo.AdmissionScore)),
            StatusCode.Ok, true,
            ResponseMessages.Success);
    }

    public async Task<GetStudentResponse> SearchAsync(string? searchString)
    {
        var users = await userManager.GetUsersInRoleAsync("Student");
        if (users.Count is 0)
        {
            return new GetStudentResponse([], StatusCode.NotFound, false, ResponseMessages.NotFound);
        }

        if (searchString is null)
        {
            return new GetStudentResponse([], StatusCode.Ok, true, ResponseMessages.Success);
        }

        var filteredUsers = users.Where(u =>
            u.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase) ||
            u.Surname.Contains(searchString, StringComparison.CurrentCultureIgnoreCase) ||
            u.MiddleName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase));

        var userIds = filteredUsers.Select(u => u.Id).ToList();

        var students = await studentRepository.FindAsync(
            x => userIds.Contains(x.AppUserId),
            x => x
                .Include(st => st.StudentAcademicInfo)
                .ThenInclude(st => st.Group)
                .Include(st => st.StudentAcademicInfo.AdmissionYear)
                .Include(st => st.StudentAcademicInfo.Specialization)
                .Include(st => st.AppUser),
            false);

        return new GetStudentResponse(students.Count == 0
                ? []
                : students.Select(x => new GetStudentDto(
                    x.AppUser.Name + " " + x.AppUser.Surname + " " + x.AppUser.MiddleName,
                    x.AppUser.UserName,
                    x.StudentAcademicInfo.Group.Code,
                    GetYear(x.StudentAcademicInfo.Group.CreatedAt, DateTime.Now),
                    x.StudentAcademicInfo.Specialization.Name,
                    x.StudentAcademicInfo.AdmissionYear.FirstYear + "/" +
                    x.StudentAcademicInfo.AdmissionYear.SecondYear, x.StudentAcademicInfo.AdmissionScore
                )),
            StatusCode.Ok,
            true,
            ResponseMessages.Success
        );
    }

    public async Task<GetStudentResponse> GetAllAsync(int page, int pageSize)
    {
        var students =
            (await studentRepository.GetAllAsync(page, pageSize, false,
                x => x
                    .Include(st => st.StudentAcademicInfo)
                    .ThenInclude(st => st.Group)
                    .Include(st => st.StudentAcademicInfo.AdmissionYear)
                    .Include(st => st.StudentAcademicInfo.Specialization)
                    .Include(st => st.AppUser)))
            .Select(x =>
                new GetStudentDto(x.AppUser.Name + " " + x.AppUser.Surname + " " + x.AppUser.MiddleName,
                    x.AppUser.UserName, x.StudentAcademicInfo.Group.Code,
                    GetYear(x.StudentAcademicInfo.Group.CreatedAt, DateTime.Now),
                    x.StudentAcademicInfo.Specialization.Name,
                    x.StudentAcademicInfo.AdmissionYear.FirstYear + "/" +
                    x.StudentAcademicInfo.AdmissionYear.SecondYear, x.StudentAcademicInfo.AdmissionScore));
        return new GetStudentResponse(students, StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<MarkAbsenceStudentResponse> MarkAbsenceAsync(string studentId, string classId)
    {
        var student =
            await studentRepository.GetByIdAsync(studentId, include: x => x
                .Include(e => e.StudentAcademicInfo)
                .ThenInclude(e => e.Group)
                .Include(e => e.Attendances), tracking: true);
        if (student is null)
        {
            return new MarkAbsenceStudentResponse(StatusCode.NotFound, false,
                $"Student with id {studentId} not found ");
        }

        var attendance = student.Attendances.FirstOrDefault(x => x.ClassId == classId);
        if (attendance is null)
        {
            return new MarkAbsenceStudentResponse(StatusCode.NotFound, false, $"Class with id {classId} not found ");
        }

        var res = await attendanceService.UpdateAttendanceAsync(attendance);
        if (!res.IsSucceeded)
        {
            return new MarkAbsenceStudentResponse(StatusCode.InternalServerError, false,
                $"Attendance status of the student with an Id of {student.Id} couldn't be updated");
        }

        return new MarkAbsenceStudentResponse(StatusCode.Ok, true,
            $"Attendance status of the student with an Id of {student.Id} updated successfully");
    }

    public async Task<GradeStudentColloquiumResponse>
        GradeStudentColloquiumAsync(GradeStudentColloquiumRequest request)
    {
        // var student = await studentRepository.GetByIdAsync(request.StudentId, tracking: true);
        var colloquium = await colloquiumRepository.GetByIdAsync(request.ColloquiumId, tracking: true);
        if (colloquium is null)
        {
            return new GradeStudentColloquiumResponse(StatusCode.BadRequest, false,
                $"Colloquium with an Id of {request.ColloquiumId} not found");
        }

        colloquium.Grade = request.Grade;
        return await colloquiumRepository.UpdateAsync(colloquium)
            ? new GradeStudentColloquiumResponse(StatusCode.Ok, true, ResponseMessages.Success)
            : new GradeStudentColloquiumResponse(StatusCode.InternalServerError, false,
                "An error occured while updating the grade");
    }

    public async Task<GradeStudentIndependentWorkResponse> GradeIndependentWorkAsync(
        GradeIndependentWorkRequest request)
    {
        var independentWork = await independentWorkRepository.GetByIdAsync(request.IndependentWorkId, tracking: true);
        if (independentWork is null)
        {
            return new GradeStudentIndependentWorkResponse(StatusCode.BadRequest, false,
                $"independent work with an Id of {request.IndependentWorkId} not found");
        }

        independentWork.IsPassed = request.IsPassed;
        return await independentWorkRepository.UpdateAsync(independentWork)
            ? new GradeStudentIndependentWorkResponse(StatusCode.Ok, true, ResponseMessages.Success)
            : new GradeStudentIndependentWorkResponse(StatusCode.InternalServerError, false,
                "An error occured while updating the grade");
    }

    public async Task<GradeStudentSeminarResponse> GradeSeminarAsync(GradeSeminarRequest request)
    {
        var colloquium = await seminarRepository.GetByIdAsync(request.SeminarId, tracking: true);
        if (colloquium is null)
        {
            return new GradeStudentSeminarResponse(StatusCode.BadRequest, false,
                $"Seminar with an Id of {request.SeminarId} not found");
        }

        colloquium.Grade = request.Grade;
        return await seminarRepository.UpdateAsync(colloquium)
            ? new GradeStudentSeminarResponse(StatusCode.Ok, true, ResponseMessages.Success)
            : new GradeStudentSeminarResponse(StatusCode.InternalServerError, false,
                "An error occured while updating the grade");
    }

    public async Task<ApiResult<GetIndependentWorksDto>> GetIndependentWorksByUserIdAsync(string studentId,
        string taughtSubjectId)
    {
        var student =
            await studentRepository.GetByIdAsync(studentId, include: x => x
                .Include(e => e.StudentAcademicInfo)
                .ThenInclude(e => e.Group)
                .Include(e => e.Attendances), tracking: true);
        if (student is null)
        {
            return ApiResult<GetIndependentWorksDto>.BadRequest($"Student with id {studentId} not found ");
        }

        if (!await taughtSubjectRepository.AnyAsync(ts => ts.Id == taughtSubjectId))
        {
            return ApiResult<GetIndependentWorksDto>.BadRequest($"Taught Subject with id {taughtSubjectId} not found ");
        }

        var independentWorks = await independentWorkRepository.FindAsync(independentWork =>
                independentWork.StudentId == student.Id && taughtSubjectId == independentWork.TaughtSubjectId,
            tracking: false);

        return ApiResult<GetIndependentWorksDto>.Success(new GetIndependentWorksDto(
                independentWorks.Count > 0
                    ? independentWorks.Select(x => new GetIndependentWorkDto(x!.Id, x.Number, x.IsPassed)).ToList()
                    : []
            )
        );
    }

    private static int ApplyAttendancePenalty(int hours, int attendances, int assignmentScore)
    {
        if (!AttendanceRules.TryGetValue(hours, out var rule))
            throw new ArgumentException("Invalid subject hours");

        if (attendances >= rule.forbidden)
            return 0; // buraxılmır

        if (attendances >= rule.twoPoint)
            return Math.Max(0, assignmentScore - 2);

        if (attendances >= rule.onePoint)
            return Math.Max(0, assignmentScore - 1);

        return assignmentScore;
    }

    private static double CalculateOverallSubjectScore(
        List<int> seminarScores,
        List<int> colloquiumScores,
        Grade assignment,
        int hours,
        int attendances)
    {
        double seminarAvg = seminarScores.Count != 0
            ? seminarScores.Average()
            : 0;

        double colloquiumAvg = colloquiumScores.Count != 0
            ? colloquiumScores.Average()
            : 0;

        int finalAssignmentScore =
            ApplyAttendancePenalty(hours, attendances, (int)assignment);

        return (colloquiumAvg * 1.5)
               + (seminarAvg * 1.5)
               + finalAssignmentScore;
    }


    private static int GetToday()
        => (int)DateTime.Today.DayOfWeek;

    private static int GetYear(DateTime start, DateTime end)
    {
        var years = (end - start).TotalDays / 365.2425;
        return years < 1 ? 1 : (int)years;
    }
}