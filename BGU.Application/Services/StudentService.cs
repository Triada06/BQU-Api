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
        new() // to calculate Einstein GPA
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
            s => s
                .Include(ai => ai.Group)
                .ThenInclude(g => g.TaughtSubjects)
                .ThenInclude(ts => ts.Subject)
                .Include(st => st.Group.TaughtSubjects)
                .ThenInclude(ts => ts.Teacher)
                .ThenInclude(t => t.AppUser)
                .Include(st => st.Group.TaughtSubjects)
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

        var classesToday = student.Group.TaughtSubjects
            .SelectMany(gs => gs.Classes)
            .Where(c => c.ClassTime.ClassDate.DateTime.Date == DateTime.Today)
            .Select(c => new TodaysClassesDto(
                c.Id,
                c.TaughtSubjectId,
                c.TaughtSubject.Subject.Name,
                c.ClassType.ToString(),
                c.TaughtSubject.Teacher.AppUser.Name,
                c.ClassTime.Start,
                c.ClassTime.End,
                new DateTimeOffset(DateTime.Today.Add(c.ClassTime.Start)),
                c.Room, c.TaughtSubject.Code,
                c.ClassTime.IsUpperWeek
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
            s => s
                .Include(ai => ai.Group)
                .ThenInclude(g => g.TaughtSubjects)
                .ThenInclude(ts => ts.Subject)
                .Include(st => st.Group.TaughtSubjects)
                .ThenInclude(ts => ts.Teacher)
                .ThenInclude(t => t.AppUser)
                .Include(st => st.Group.TaughtSubjects)
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

        var todayDate = DateTime.Today;
        int diff = (7 + (todayDate.DayOfWeek - DayOfWeek.Monday)) % 7;
        var weekStart = todayDate.AddDays(-diff);
        var weekEnd = weekStart.AddDays(6);

        if (request.Schedule == "week")
        {
            var classesThisWeek = student.Group.TaughtSubjects
                .SelectMany(gs => gs.Classes)
                .Where(c =>
                    c.ClassTime.ClassDate.Date >= weekStart &&
                    c.ClassTime.ClassDate.Date <= weekEnd)
                .Select(c =>
                {
                    var classDateTime = c.ClassTime.ClassDate.Date.Add(c.ClassTime.Start);
                    return new TodaysClassesDto(
                        c.Id,
                        c.TaughtSubjectId,
                        c.TaughtSubject.Subject.Name,
                        c.ClassType.ToString(),
                        c.TaughtSubject.Teacher.AppUser.Name,
                        c.ClassTime.Start,
                        c.ClassTime.End,
                        new DateTimeOffset(classDateTime),
                        c.Room,
                        c.TaughtSubject.Code,
                        c.ClassTime.IsUpperWeek ?? CheckIfUpperWeek()
                    );
                })
                .OrderBy(c => c.Period)
                .ToList();

            return new StudentScheduleResponse(
                new StudentScheduleDto(DateTime.Now.ToString("dddd, MMM dd"), CheckIfUpperWeek(), classesThisWeek),
                "Found", true, 200);
        }

        var classesToday = student.Group.TaughtSubjects
            .SelectMany(gs => gs.Classes)
            .Where(c => c.ClassTime.ClassDate.Date == todayDate)
            .Select(c =>
            {
                var classDateTime = c.ClassTime.ClassDate.Date.Add(c.ClassTime.Start);
                return new TodaysClassesDto(
                    c.Id,
                    c.TaughtSubjectId,
                    c.TaughtSubject.Subject.Name,
                    c.ClassType.ToString(),
                    c.TaughtSubject.Teacher.AppUser.Name,
                    c.ClassTime.Start,
                    c.ClassTime.End,
                    new DateTimeOffset(classDateTime),
                    c.Room,
                    c.TaughtSubject.Code,
                    c.ClassTime.IsUpperWeek ?? CheckIfUpperWeek()
                );
            })
            .OrderBy(c => c.Period)
            .ToList();

        return new StudentScheduleResponse(
            new StudentScheduleDto(DateTime.Now.ToString("dddd, MMM dd"), CheckIfUpperWeek(), classesToday),
            "Found", true, 200);
    }

    public async Task<StudentGradesResponse> GetGrades(string userId, StudentGradesRequest request)
    {
        var studentData = await studentRepository.Table
            .Where(s => s.AppUserId == userId)
            .Select(s => new
            {
                s.Id,
                GroupCode = s.Group.Code,
                TaughtSubjects = s.Group.TaughtSubjects.Select(ts => new
                {
                    ts.Id,
                    ts.Hours,
                    SubjectName = ts.Subject.Name,
                    ts.Subject.CreditsNumber,
                    TeacherName = ts.Teacher.AppUser.Name,
                    Classes = ts.Classes.Select(c => new
                    {
                        c.Id,
                        c.ClassType,
                        c.ClassTime.ClassDate
                    }).ToList(),
                    Seminars = ts.Seminars.Where(sem => sem.StudentId == s.Id && sem.Grade != Grade.None).ToList(),
                    IndependentWorks = ts.IndependentWorks.Where(iw => iw.StudentId == s.Id).ToList(),
                }).ToList(),
                Attendances = s.Attendances.Select(a => new { a.ClassId, a.IsPresent, a.Date }).ToList(),
                Colloquiums = s.Colloquiums.Where(col => col.Grade != Grade.None).OrderBy(x => x.OrderNumber).ToList(),
            })
            .FirstOrDefaultAsync();
//TODO: FIX ORDERING IN ATTENDENCAS AT STUDENT GRADES
        if (studentData == null)
        {
            return new StudentGradesResponse(null, null, ResponseMessages.NotFound, false, 404);
        }

        // var attendanceMap = studentData.Attendances.ToLookup(a => a.ClassId);
        //
        // if (request.Grade == "sessions")
        // {
        //     var sessions = studentData.TaughtSubjects.Select(ts => new ClassSessions(
        //         ts.SubjectName,
        //         ts.Classes.Select(c => new ClassInfo(
        //             c.ClassDate,
        //             c.ClassType.ToString(),
        //             attendanceMap[c.Id].FirstOrDefault()?.IsPresent ?? true,
        //             c.ClassType == ClassType.Семинар
        //                 ? (int?)ts.Seminars.FirstOrDefault(sg => sg.GotAt == c.ClassDate)?.Grade
        //                 : null)
        //         ))
        //     ).ToList();
        //
        //     return new StudentGradesResponse(null, sessions, "Ok", true, 200);
        // }

        var performance = studentData.TaughtSubjects.Select(ts =>
        {
            var seminarGrades = ts.Seminars.Select(s => (int)s.Grade).ToList();
            var colloquiumGrades = studentData.Colloquiums
                .Where(coll => coll.TaughtSubjectId == ts.Id)
                .Select(s => (int)s.Grade).ToList();
            var passedIwCount = ts.IndependentWorks.Count(iw => iw.IsPassed is true);

            var subjectClassIds = ts.Classes.Select(c => c.Id).ToList();
            var subjectAttendanceCount = studentData.Attendances.Count(a => subjectClassIds.Contains(a.ClassId));
            var subjectAbsenceCount =
                studentData.Attendances.Where(a => subjectClassIds.Contains(a.ClassId)).OrderBy(x => x.Date)
                    .Select(x => x.IsPresent);

            return new AcademicPerformanceDto(
                ts.SubjectName,
                studentData.GroupCode,
                ts.TeacherName,
                ts.CreditsNumber,
                ts.Hours,
                CalculateOverallSubjectScore(
                    seminarGrades,
                    colloquiumGrades,
                    (Grade)passedIwCount,
                    ts.Hours,
                    subjectAttendanceCount),
                seminarGrades,
                colloquiumGrades,
                ts.IndependentWorks.Select(iw => new GetIndependentWorkDto(iw.Id, iw.Number, iw.IsPassed)),
                subjectAbsenceCount,
                ts.Classes.Count
            );
        });

        return new StudentGradesResponse(new StudentGradesDto(performance), null, "Ok", true, 200);
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
            s => s
                .Include(ai => ai.Group)
                .ThenInclude(g => g.TaughtSubjects)
                .ThenInclude(ts => ts.Subject)
                .Include(st => st.AdmissionYear)
                .Include(st => st.Faculty)
                .Include(st => st.Specialization)
                .Include(st => st.Group.TaughtSubjects)
                .ThenInclude(ts => ts.Teacher)
                .ThenInclude(t => t.AppUser)
                .Include(st => st.Group.TaughtSubjects)
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
            student.Gpa, nameof(student.Group.EducationLevel),
            student.AdmissionYear.FirstYear, student.Faculty.Name,
            student.Specialization.Name,user.Email);

        return new StudentProfileResponse(studentAcademicInfo, ResponseMessages.Success, true,
            (int)StatusCode.Ok);
    }

    public async Task<GetStudentResponse> FilterAsync(string? groupId, int? year)
    {
        //TODO: fix filter, shouldnt be hardcoded with 1000 pagesize
        var students = (await studentRepository.GetAllAsync(1, 10000, false, x => x
            .Include(st => st.Group)
            .Include(st => st.AdmissionYear)
            .Include(st => st.Specialization)
            .Include(st => st.AppUser))).ToList();
        if (students.Count == 0)
        {
            return new GetStudentResponse([], StatusCode.Ok, true, ResponseMessages.Success);
        }

        if (groupId is not null)
        {
            students = students.Where(x => x.GroupId == groupId).ToList();
        }

        if (year is not null && students.Count is not 0)
        {
            students = students.Where(x => GetYear(x.AdmissionYear.FirstYear) == year)
                .ToList();
        }

        return new GetStudentResponse(students.Count == 0
                ? []
                : students.Select(x => new GetStudentDto(x.Id,
                    x.AppUser.Name + " " + x.AppUser.Surname + " " + x.AppUser.MiddleName, x.AppUser.UserName,
                    x.Group.Code,
                    GetYear(x.AdmissionYear.FirstYear),
                    x.Specialization.Name,
                    x.AdmissionYear.FirstYear + "/" +
                    x.AdmissionYear.SecondYear, x.AdmissionScore)),
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
                .Include(st => st.Group)
                .Include(st => st.AdmissionYear)
                .Include(st => st.Specialization)
                .Include(st => st.AppUser),
            false);

        return new GetStudentResponse(students.Count == 0
                ? []
                : students.Select(x => new GetStudentDto(x.Id,
                    x.AppUser.Name + " " + x.AppUser.Surname + " " + x.AppUser.MiddleName,
                    x.AppUser.UserName,
                    x.Group.Code,
                    GetYear(x.AdmissionYear.FirstYear),
                    x.Specialization.Name,
                    x.AdmissionYear.FirstYear + "/" +
                    x.AdmissionYear.SecondYear, x.AdmissionScore
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
                    .Include(st => st.Group)
                    .Include(st => st.AdmissionYear)
                    .Include(st => st.Specialization)
                    .Include(st => st.AppUser)))
            .Select(x =>
                new GetStudentDto(x.Id,x.AppUser.Name + " " + x.AppUser.Surname + " " + x.AppUser.MiddleName,
                    x.AppUser.UserName, x.Group.Code,
                    GetYear(x.AdmissionYear.FirstYear),
                    x.Specialization.Name,
                    x.AdmissionYear.FirstYear + "/" +
                    x.AdmissionYear.SecondYear, x.AdmissionScore));
        return new GetStudentResponse(students, StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<MarkAbsenceStudentResponse> MarkAbsenceAsync(string studentId, string classId)
    {
        var student =
            await studentRepository.GetByIdAsync(studentId, include: x => x
                .Include(e => e.Group)
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
                .Include(e => e)
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

    public async Task<ApiResult<GetStudentPageDto>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var student = (await studentRepository.FindAsync(
            s => s.Id == id,
            s => s
                .Include(ai => ai.Group)
                .ThenInclude(g => g.TaughtSubjects)
                .ThenInclude(ts => ts.Subject)
                .Include(st => st.Group.TaughtSubjects)
                .ThenInclude(ts => ts.Teacher)
                .ThenInclude(t => t.AppUser)
                .Include(st => st.Group.TaughtSubjects)
                .ThenInclude(ts => ts.Classes)
                .ThenInclude(c => c.ClassTime)
                .Include(x=>x.AdmissionYear)
                .Include(x=>x.Specialization)
                .Include(x=>x.AppUser)
        )).FirstOrDefault();
        
        if (student is null)
        {
            return new ApiResult<GetStudentPageDto>
            {
                Data = null,
                Message = $"Student with an Id of {id} not found",
                IsSucceeded = false,
                StatusCode = 404
            };
        }

        var formattedAdmissionYear = $"{student.AdmissionYear.FirstYear}/{student.AdmissionYear.SecondYear}";

        var classesToday = student.Group.TaughtSubjects
            .SelectMany(gs => gs.Classes)
            .Where(c => c.ClassTime.ClassDate.Date == DateTime.Today)
            .Select(c =>
            {
                var classDateTime = c.ClassTime.ClassDate.Date.Add(c.ClassTime.Start);
                return new TodaysClassesDto(
                    c.Id,
                    c.TaughtSubjectId,
                    c.TaughtSubject.Subject.Name,
                    c.ClassType.ToString(),
                    c.TaughtSubject.Teacher.AppUser.Name,
                    c.ClassTime.Start,
                    c.ClassTime.End,
                    new DateTimeOffset(classDateTime),
                    c.Room,
                    c.TaughtSubject.Code,
                    c.ClassTime.IsUpperWeek ?? CheckIfUpperWeek()
                );
            })
            .OrderBy(c => c.Period)
            .ToList();

        var data = new GetStudentPageDto(student.Group.Code, student.Specialization.Name, formattedAdmissionYear,
            GetYear(student.AdmissionYear.FirstYear), student.AdmissionScore, student.AppUser.Email, classesToday);

        return new ApiResult<GetStudentPageDto>
        {
            Data = data,
            Message = ResponseMessages.Success,
            IsSucceeded = true,
            StatusCode = 200
        };
    }

    //TODO: GPA IS NOT BEING STORED IN THE DATABASE, FIX REQUIRED
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

    // This method returns current course of the group/student
    private static int GetYear(int firstYear) //2023
    {
        var currentYear = DateTime.Today.Year;
        var currentMonth = DateTime.Today.Month;
        var data = currentMonth >= 9 ? currentYear - firstYear - 1 : currentYear - firstYear;
        return data;
    }

    private static bool CheckIfUpperWeek()
    {
        var today = DateTime.Today;

        var semesterStart = today.Month >= 9
            ? new DateTime(today.Year, 9, 14)
            : new DateTime(today.Year, 2, 14);

        // Find the first Monday of the semester
        var startDayOfWeek = (int)semesterStart.DayOfWeek;
        if (startDayOfWeek == 0) startDayOfWeek = 7;
        var daysUntilMonday = startDayOfWeek == 1 ? 0 : (8 - startDayOfWeek);
        var firstMonday = semesterStart.AddDays(daysUntilMonday);

        // Find this week's Monday
        var todayDayOfWeek = (int)today.DayOfWeek;
        if (todayDayOfWeek == 0) todayDayOfWeek = 7;
        var currentMonday = today.AddDays(-(todayDayOfWeek - 1));

        var weeksPassed = (int)((currentMonday - firstMonday).TotalDays / 7);

        // Week 0, 2, 4... = upper; Week 1, 3, 5... = lower
        return weeksPassed % 2 == 0;
    }
}