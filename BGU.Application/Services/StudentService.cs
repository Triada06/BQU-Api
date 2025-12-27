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
    ITaughtSubjectRepository taughtSubjectRepository,
    IAttendanceService attendanceService,
    IColloquiumRepository colloquiumRepository) : IStudentService
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
            // var sessions = student.StudentAcademicInfo.Group.TaughtSubjects
            //     .Select(c => new ClassSessions(
            //         c.Subject.Name,
            //         c.ClassSessions.Select(e => new ClassInfo(e.Date,
            //             c.Classes.Where(x => x.CreatedAt == e.Date).Select(m => m.ClassType).First(),
            //             e.Attendances.Where(x => x.CreatedAt == e.Date).Select(m => m.IsAbsent).First(),
            //             student.SeminarGrades.Where(ex => ex.GotAt == e.Date).Select(m => (int)m.Grade).First())
            //         ))
            //     )
            //     .ToList();
            // return new StudentGradesResponse(null, sessions,
            //     "Ok", true, 200);
        }

        var taughtSubjects = student.StudentAcademicInfo.Group.TaughtSubjects;

        // var classes = taughtSubjects
        //     .Select(c => new AcademicPerformanceDto(
        //         c.Subject.Name,
        //         c.Group.Code,
        //         c.Teacher.AppUser.Name,
        //         c.Subject.CreditsNumber,
        //         c.Hours,
        //         CalculateOverallSubjectScore(
        //             c.Seminars.Where(x => x.StudentId == student.Id).Select(s => (int)s.Grade)
        //                 .ToList(),
        //             c.Colloquiums.Where(x => x.StudentId == student.Id).Select(s => (int)s.Grade)
        //                 .ToList(),
        //             (Grade)c.IndependentWorks.Where(x => x.StudentId == student.Id)
        //                 .Count(s => s.IsPassed)),
        //         c.Seminars.Where(x => x.StudentId == student.Id).Select(s => (int)s.Grade)
        //             .ToList(),
        //         c.Colloquiums.Where(x => x.StudentId == student.Id).Select(s => (int)s.Grade)
        //             .ToList(),
        //         c.IndependentWorks.Where(x => x.StudentId == student.Id)
        //             .Count(s => s.IsPassed),
        //         c.ClassSessions
        //             .Select(m => m.Attendances.Where(x => x.StudentId == student.Id).Select(x => x.IsAbsent)).Count(),
        //         c.Hours / 2)
        //     )
        //     .ToList();
        // return new StudentGradesResponse(new StudentGradesDto(classes), null,
            // "Ok", true, 200);
        return new StudentGradesResponse(new StudentGradesDto([]), null,
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
                : students.Select(x => new GetStudentDto(x.Id,
                    x.AppUser.Name + "  " + x.AppUser.Surname, x.StudentAcademicInfo.Group.Code,
                    GetYear(x.StudentAcademicInfo.Group.CreatedAt, DateTime.Now), x.AppUser.Gender,
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
                    x!.Id,
                    x.AppUser.Name + " " + x.AppUser.Surname,
                    x.StudentAcademicInfo.Group.Code,
                    GetYear(x.StudentAcademicInfo.Group.CreatedAt, DateTime.Now), x.AppUser.Gender,
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
                new GetStudentDto(x.Id, x.AppUser.Name + "  " + x.AppUser.Surname, x.StudentAcademicInfo.Group.Code,
                    GetYear(x.StudentAcademicInfo.Group.CreatedAt, DateTime.Now), x.AppUser.Gender,
                    x.StudentAcademicInfo.Specialization.Name,
                    x.StudentAcademicInfo.AdmissionYear.FirstYear + "/" +
                    x.StudentAcademicInfo.AdmissionYear.SecondYear, x.StudentAcademicInfo.AdmissionScore));
        return new GetStudentResponse(students, StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<MarkAbsenceStudentResponse> MarkAbsenceAsync(string studentId, string teacherId,
        string taughtSubjectId, string classId)
    {
        var student =
            await studentRepository.GetByIdAsync(studentId, include: x => x
                .Include(e => e.StudentAcademicInfo)
                .ThenInclude(e => e.Group), tracking: true);
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

    public async Task<GradeStudentColloquiumResponse> GradeStudentColloquiumAsync(GradeStudentColloquiumRequest request)
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

    private static double CalculateOverallSubjectScore(List<int> seminarScores, List<int> colloquiumScores,
        Grade assigment)
        => (colloquiumScores.Sum() / 0.4) + seminarScores.Sum() / 0.6 + (int)assigment;

    private static int GetToday()
        => (int)DateTime.Today.DayOfWeek;

    private static int GetYear(DateTime start, DateTime end)
    {
        var years = (end - start).TotalDays / 365.2425;
        return years < 1 ? 1 : (int)years;
    }
}