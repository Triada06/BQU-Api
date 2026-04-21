using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
using BGU.Application.Common;
using BGU.Application.Contracts.TaughtSubjects.Requests;
using BGU.Application.Contracts.TaughtSubjects.Responses;
using BGU.Application.Dtos.Class;
using BGU.Application.Dtos.Student;
using BGU.Application.Dtos.TaughtSubject;
using BGU.Application.Dtos.TaughtSubject.Requests;
using BGU.Application.Dtos.TaughtSubject.Responses;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Core.Enums;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class TaughtSubjectService(
    IGroupRepository groupRepository,
    ITaughtSubjectRepository taughtSubjectRepository,
    ISubjectRepository subjectRepository,
    IClassTimeRepository classTimeRepository,
    ISyllabusService syllabusService,
    ISyllabusRepository syllabusRepository,
    IClassRepository classRepository,
    IColloquiumRepository colloquiumRepository,
    IStudentRepository studentRepository,
    IAttendanceRepository attendanceRepository,
    ISeminarRepository seminarRepository,
    IIndependentWorkRepository independentWorkRepository,
    AppDbContext context) : ITaughtSubjectService
{
    public async Task<DeleteTaughtSubjectResponse> DeleteAsync(string id)
    {
        var taughtSubject = await taughtSubjectRepository.GetByIdAsync(id, tracking: true);
        if (taughtSubject is null)
        {
            return new DeleteTaughtSubjectResponse(StatusCode.NotFound, false, ResponseMessages.NotFound);
        }

        if (!await taughtSubjectRepository.DeleteAsync(taughtSubject))
        {
            return new DeleteTaughtSubjectResponse(StatusCode.InternalServerError, false, ResponseMessages.Failed);
        }

        return new DeleteTaughtSubjectResponse(StatusCode.Ok, false, ResponseMessages.Success);
    }

    public async Task<UpdateTaughtSubjectResponse> UpdateAsync(string id, UpdateTaughtSubjectRequest taughtSubject)
    {
        var subjectToUpdate =
            await taughtSubjectRepository.GetByIdAsync(id, include: i => i.Include(x => x.Subject), tracking: true);
        if (subjectToUpdate is null)
        {
            return new UpdateTaughtSubjectResponse(null, StatusCode.NotFound, false, ResponseMessages.NotFound);
        }

        subjectToUpdate.Subject.CreditsNumber = taughtSubject.Credits;
        subjectToUpdate.Code = taughtSubject.Code;
        subjectToUpdate.Subject.DepartmentId = taughtSubject.DepartmentId;
        subjectToUpdate.GroupId = taughtSubject.GroupId;
        subjectToUpdate.Subject.Name = taughtSubject.Title;
        subjectToUpdate.TeacherId = taughtSubject.TeacherId;
        if (!await taughtSubjectRepository.UpdateAsync(subjectToUpdate))
        {
            return new UpdateTaughtSubjectResponse(null, StatusCode.InternalServerError, false,
                ResponseMessages.Failed);
        }

        return new UpdateTaughtSubjectResponse(subjectToUpdate.Id,
            StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<GetAllTaughtSubjectResponse> GetAllAsync(int page, int pageSize, bool tracking = false)
    {
        var subjects =
            (await taughtSubjectRepository.GetAllPaginatedAsync(null, page, pageSize: pageSize,
                include: i => i
                    .Include(x => x.Group)
                    .ThenInclude(x => x.AdmissionYear)
                    .Include(x => x.Subject)
                    .ThenInclude(x => x.Department)
                    .Include(x => x.Teacher)
                    .ThenInclude(x => x.AppUser),
                tracking: false)).Items.Select(x =>
                new GetTaughtSubjectDto(x.Id, x.Code, x.Subject.Name,
                    x.Subject.Department.Name,
                    GetYear(x.Group.AdmissionYear.FirstYear),
                    x.Teacher.AppUser.Name, x.Group.Code,
                    x.Subject.CreditsNumber));

        return new GetAllTaughtSubjectResponse(subjects, StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<GetByIdTaughtSubjectResponse> GetByIdAsync(string id, bool tracking = false)
    {
        var subject =
            await taughtSubjectRepository.GetByIdAsync(id, include: i => i
                .Include(x => x.Group)
                .ThenInclude(x => x.AdmissionYear)
                .Include(x => x.Subject)
                .ThenInclude(x => x.Department)
                .Include(x => x.Teacher)
                .ThenInclude(x => x.AppUser), tracking: false);

        if (subject is null)
        {
            return new GetByIdTaughtSubjectResponse(null, StatusCode.NotFound, false, ResponseMessages.NotFound);
        }

        return new GetByIdTaughtSubjectResponse(new GetTaughtSubjectDto(subject.Id, subject.Code,
            subject.Subject.Name,
            subject.Subject.Department.Name,
            GetYear(subject.Group.AdmissionYear.FirstYear),
            subject.Teacher.AppUser.Name,
            subject.Group.Code,
            subject.Subject.CreditsNumber), StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<CreateTaughtSubjectResponse> CreateAsync(CreateTaughtSubjectRequest request)
    {
        if (await taughtSubjectRepository.AnyAsync(x => x.Code == request.Code))
        {
            return new CreateTaughtSubjectResponse(null, false, StatusCode.Conflict,
                "Course with this name already exists.");
        }

        var subject =
            (await subjectRepository.FindAsync(x => x.Name.ToLower().Trim() == request.Title.ToLower().Trim(),
                tracking: true))
            .FirstOrDefault();
        if (subject == null)
        {
            subject = new Subject
            {
                CreditsNumber = request.Credits,
                Name = request.Title,
                DepartmentId = request.DepartmentId,
            };
            if (!await subjectRepository.CreateAsync(subject))
            {
                return new CreateTaughtSubjectResponse(null, false, StatusCode.InternalServerError,
                    "Failed to create subject");
            }
        }

        var group = await groupRepository.GetByIdAsync(request.GroupId, include: x => x.Include(g => g.AdmissionYear));
        if (group is null)
        {
            return new CreateTaughtSubjectResponse(null, false, StatusCode.BadRequest, ResponseMessages.BadRequest);
        }

        var attendanceYear = DateTime.Now.Month >= 9 //this calculates the current study year of the group
            ? GetAttendanceYear(group.AdmissionYear.FirstYear - 1)
            : GetAttendanceYear(group.AdmissionYear.FirstYear);

        if (request.Year < attendanceYear ||
            (request.Year == attendanceYear && request.Semester != GetSemester()))
        {
            return new CreateTaughtSubjectResponse(
                null,
                false,
                StatusCode.BadRequest,
                "Year can't be older than the group's current year"
            );
        }


        var taughtSubject = new TaughtSubject
        {
            Code = request.Code,
            TeacherId = request.TeacherId,
            GroupId = request.GroupId,
            SubjectId = subject.Id,
            Hours = request.Hours,
        };

        if (!await taughtSubjectRepository.CreateAsync(taughtSubject))
        {
            return new CreateTaughtSubjectResponse(null, false, StatusCode.InternalServerError,
                "Something went wrong while creating the course");
        }

        var (classes, classTimes) = GenerateClassesAndClassTimes(
            group.AdmissionYear,
            request.Hours,
            request.ClassTimes,
            request.Year,
            request.Semester,
            taughtSubject.Id
        );

        if (!await classRepository.BulkCreateWithTimesAsync(classes, classTimes))
        {
            return new CreateTaughtSubjectResponse(null, false, StatusCode.InternalServerError,
                "Something went wrong while creating classes");
        }

        taughtSubject.Classes = classes;

        //creating an attendance system
        var studentsInGroup =
            await studentRepository.FindAsync(st => st.GroupId == request.GroupId);
        var seminarTypes = classes.FindAll(x => x.ClassType == ClassType.Семинар);

        if (studentsInGroup.Count != 0 && studentsInGroup.All(x => x is not null))
        {
            var attendances = new List<Attendance>();
            var seminars = new List<Seminar>();
            var independentWorks = new List<IndependentWork>();
            var colloquiums = new List<Colloquiums>();

            if (seminarTypes.Count != 0)
            {
                foreach (var seminarType in seminarTypes)
                {
                    seminars.AddRange(studentsInGroup.Where(s => s != null).Select(student => new Seminar
                    {
                        StudentId = student.Id,
                        TaughtSubjectId = seminarType.TaughtSubjectId,
                        GotAt = seminarType.ClassTime.ClassDate.UtcDateTime,
                        Grade = Grade.None
                    }));
                }

                if (!await seminarRepository.BulkCreate(seminars))
                {
                    return new CreateTaughtSubjectResponse(null, false, StatusCode.InternalServerError,
                        "Failed to create seminars");
                }
            }

            // For EACH class, create attendance for EACH student
            foreach (var classItem in classes)
            {
                attendances.AddRange(studentsInGroup.Select(student => new Attendance
                    { StudentId = student!.Id, ClassId = classItem.Id, IsPresent = false }));
            }


            if (!await attendanceRepository.BulkCreateAsync(attendances))
            {
                return new CreateTaughtSubjectResponse(null, false, StatusCode.InternalServerError,
                    "Failed to create attendances");
            }

            //create independent works
            foreach (var student in studentsInGroup)
            {
                for (int i = 0; i < 5; i++)
                {
                    var independentWork = new IndependentWork
                    {
                        Number = i + 1,
                        StudentId = student!.Id,
                        TaughtSubjectId = taughtSubject.Id,
                        Grade = Grade.None
                    };
                    independentWorks.Add(independentWork);
                }
            }

            if (!await independentWorkRepository.BulkCreateAsync(independentWorks))
            {
                return new CreateTaughtSubjectResponse(null, false, StatusCode.InternalServerError,
                    "Failed to create independent works (assignments)");
            }

            //create colls
            foreach (var student in studentsInGroup)
            {
                for (int i = 0; i < 3; i++)
                {
                    var coll = new Colloquiums
                    {
                        OrderNumber = i + 1,
                        Grade = Grade.None,
                        StudentId = student!.Id,
                        TaughtSubjectId = taughtSubject.Id,
                    };
                    colloquiums.Add(coll);
                }
            }

            if (!await colloquiumRepository.BulkCreateAsync(colloquiums))
            {
                return new CreateTaughtSubjectResponse(null, false, StatusCode.InternalServerError,
                    "Failed to create independent works (assignments)");
            }
        }


        return new CreateTaughtSubjectResponse(taughtSubject.Id, true, StatusCode.Ok, ResponseMessages.Success);
    }

    public async Task<ApiResult<GetActivitiesAndAttendances>> GetStudentsAndAttendances(string taughtSubjectId)
    {
        var subject = await context.TaughtSubjects
            .AsNoTracking()
            .Where(x => x.Id == taughtSubjectId)
            .Select(x => new
            {
                x.Id,
                Students = x.Group.Students.Select(s => new
                {
                    s.Id,
                    s.AppUser.Name,
                    s.AppUser.Surname
                }).ToList(),
                Classes = x.Classes.Select(c => new
                    {
                        c.Id,
                        c.ClassType,
                        c.ClassTime.ClassDate,
                        c.ClassTime.Start,
                        c.ClassTime.End
                    })
                    .OrderBy(c => c.ClassDate)
                    .ThenBy(c => c.Start)
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (subject is null)
            return ApiResult<GetActivitiesAndAttendances>.BadRequest(
                new GetActivitiesAndAttendances([]),
                $"Taught Subject with id '{taughtSubjectId}' not found");

        if (subject.Students.Count == 0)
            return ApiResult<GetActivitiesAndAttendances>.Success(
                new GetActivitiesAndAttendances([]));

        var studentIds = subject.Students.Select(s => s.Id).ToList();
        var classIds = subject.Classes.Select(c => c.Id).ToList();

        // single query for all attendances
        var allAttendances = await context.Attendances
            .AsNoTracking()
            .Where(a => studentIds.Contains(a.StudentId) && classIds.Contains(a.ClassId))
            .Select(a => new { a.Id, a.StudentId, a.ClassId, a.IsPresent })
            .ToListAsync();

        // single query for all seminars
        var allSeminars = await context.Seminars
            .AsNoTracking()
            .Where(s => studentIds.Contains(s.StudentId) && s.TaughtSubjectId == taughtSubjectId)
            .Select(s => new { s.Id, s.StudentId, s.Grade, s.CreatedAt })
            .OrderBy(s => s.CreatedAt)
            .ToListAsync();

        // group in memory
        var attendancesByStudent = allAttendances
            .GroupBy(a => a.StudentId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var seminarsByStudent = allSeminars
            .GroupBy(s => s.StudentId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var result = new List<GetActivityAndAttendance>();

        foreach (var student in subject.Students)
        {
            attendancesByStudent.TryGetValue(student.Id, out var studentAttendances);
            seminarsByStudent.TryGetValue(student.Id, out var studentSeminars);

            studentAttendances ??= [];
            studentSeminars ??= [];

            var seminarIndex = 0;
            var managedClasses = new List<MangeClassesDto>();

            foreach (var classItem in subject.Classes)
            {
                var isSeminar = classItem.ClassType == ClassType.Семинар;

                if (isSeminar)
                {
                    var seminar = seminarIndex < studentSeminars.Count
                        ? studentSeminars[seminarIndex++]
                        : null;

                    var attendance = studentAttendances.FirstOrDefault(a => a.ClassId == classItem.Id);

                    managedClasses.Add(new MangeClassesDto(
                        classItem.Id,
                        classItem.ClassDate.UtcDateTime,
                        FormatRange(classItem.Start, classItem.End),
                        'S',
                        seminar?.Grade is Grade.None ? attendance?.Id : null,
                        seminar?.Grade is Grade.None ? attendance?.IsPresent : null,
                        seminar?.Id,
                        seminar?.Grade
                    ));
                }
                else
                {
                    var attendance = studentAttendances.FirstOrDefault(a => a.ClassId == classItem.Id);

                    managedClasses.Add(new MangeClassesDto(
                        classItem.Id,
                        classItem.ClassDate.UtcDateTime,
                        FormatRange(classItem.Start, classItem.End),
                        'L',
                        attendance?.Id,
                        attendance?.IsPresent,
                        null,
                        null
                    ));
                }
            }

            result.Add(new GetActivityAndAttendance(
                student.Id,
                $"{student.Name} {student.Surname}",
                managedClasses));
        }

        return ApiResult<GetActivitiesAndAttendances>.Success(new GetActivitiesAndAttendances(result));
    }


    public async Task<ApiResult<GetStudentsForSubject>> GetStudentsAsync(string taughtSubjectId)
    {
        var subject = await taughtSubjectRepository.GetByIdAsync(
            taughtSubjectId,
            include: i => i
                .Include(x => x.Group)
                .ThenInclude(g => g.Students)
                .ThenInclude(s => s.AppUser)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                .ThenInclude(t => t.AppUser),
            tracking: false);

        if (subject is null)
        {
            return ApiResult<GetStudentsForSubject>.BadRequest(
                new GetStudentsForSubject([]),
                $"Taught Subject with an id of {taughtSubjectId} not found");
        }

        var students = subject.Group.Students
            .OrderBy(x => x.AppUser.Surname, StringComparer.Create(new CultureInfo("az-Latn-AZ"),
                false))
            .ToList();


        var studentsDto = students
            .Select(gs =>
            {
                var u = gs.AppUser;
                return new StudentsInSubjectDto(
                    gs.Id,
                    u.Name,
                    u.Surname,
                    u.MiddleName,
                    u.UserName!,
                    subject.Group.Code
                );
            })
            .ToList();

        return ApiResult<GetStudentsForSubject>.Success(
            new GetStudentsForSubject(studentsDto));
    }

    public async Task<ApiResult<GetIndependentWorksByTaughtSubjectDto>> GetIndependentWorksByTaughtSubjectIdAsync(
        string taughtSubjectId)
    {
        var subject = await taughtSubjectRepository.GetByIdAsync(
            taughtSubjectId,
            include: i => i
                .Include(x => x.Group)
                .ThenInclude(g => g.Students)
                .Include(x => x.Group)
                .ThenInclude(g => g.Students)
                .Include(x => x.Subject)
                .Include(x => x.IndependentWorks),
            tracking: false);

        if (subject is null)
        {
            return ApiResult<GetIndependentWorksByTaughtSubjectDto>.BadRequest(
                new GetIndependentWorksByTaughtSubjectDto([]),
                $"Taught Subject with an id of {taughtSubjectId} not found");
        }

        var independentWorksOfSubject = subject.IndependentWorks.ToList();
        var stuAcademicInfos = subject.Group.Students.ToList();
        List<GetIndependentWorkByTaughtSubjectDto> independentWorks = [];

        foreach (var student in stuAcademicInfos)
        {
            var studentIndependentWorks = independentWorksOfSubject
                .Where(x => x.TaughtSubjectId == taughtSubjectId && student.Id == x.StudentId);


            independentWorks.AddRange(studentIndependentWorks.Select(independentWork =>
                new GetIndependentWorkByTaughtSubjectDto(independentWork.Id, student.Id, independentWork.Number,
                    independentWork.Grade)));
        }

        return ApiResult<GetIndependentWorksByTaughtSubjectDto>.Success(
            new GetIndependentWorksByTaughtSubjectDto(independentWorks));
    }

    public async Task<ApiResult<bool>> DeleteSyllabusAsync(string id)
    {
        var course = await taughtSubjectRepository.GetByIdAsync(id, tracking: true);
        if (course is null)
        {
            return new ApiResult<bool>
            {
                Data = false,
                Message = $"Course with an Id of {id} not found",
                IsSucceeded = false,
                StatusCode = 400
            };
        }

        var syllabus =
            (await syllabusRepository.FindAsync(x => x.TaughtSubjectId == id, tracking: true)).FirstOrDefault();
        if (syllabus is null)
        {
            return new ApiResult<bool>
            {
                Data = false,
                Message = "No syllabus found for this course",
                IsSucceeded = false,
                StatusCode = 400
            };
        }

        var deleteRes = await syllabusService.DeleteAsync(syllabus.Id);
        if (!deleteRes.IsSucceeded)
        {
            return new ApiResult<bool>
            {
                Data = false,
                Message = deleteRes.ResponseMessage,
                IsSucceeded = false,
                StatusCode = (int)deleteRes.StatusCode
            };
        }

        return new ApiResult<bool>
        {
            Data = false,
            Message = "Success",
            IsSucceeded = true,
            StatusCode = 200
        };
    }

    private static (List<Class> Classes, List<ClassTime> ClassTimes) GenerateClassesAndClassTimes(
        AdmissionYear groupAdmissionYear,
        int hours,
        CreateClassDto[] classDtos,
        int year,
        int semester,
        string taughtSubjectId)
    {
        var totalClasses = hours / 2;
        var classes = new List<Class>();
        var classTimes = new List<ClassTime>();

        var groupCurrentCourse = DateTime.Now.Month >= 9
            ? DateTime.Now.Year - groupAdmissionYear.FirstYear + 1
            : DateTime.Now.Year - groupAdmissionYear.FirstYear;

        var yearDiff = year - groupCurrentCourse;
        var yearToUse = DateTime.Now.AddYears(yearDiff).Year;

        var semesterStartDate = semester % 2 == 1
            ? new DateTimeOffset(yearToUse, 9, 14, 0, 0, 0, TimeSpan.Zero)
            : new DateTimeOffset(yearToUse, 2, 14, 0, 0, 0, TimeSpan.Zero);

        var startDayOfWeek = (int)semesterStartDate.DayOfWeek;
        if (startDayOfWeek == 0) startDayOfWeek = 7;
        var daysUntilMonday = startDayOfWeek == 1 ? 0 : (8 - startDayOfWeek);
        var firstMonday = semesterStartDate.AddDays(daysUntilMonday);

        var isLecturer = true;
        var classesCreated = 0;
        var weekNumber = 1;

        while (classesCreated < totalClasses)
        {
            var isUpperWeek = weekNumber % 2 == 1;
            var currentWeekMonday = firstMonday.AddDays((weekNumber - 1) * 7);

            foreach (var dto in classDtos)
            {
                if (classesCreated >= totalClasses) break;

                var fires = dto.Frequency == Frequency.Both ||
                            (dto.Frequency == Frequency.Upper && isUpperWeek) ||
                            (dto.Frequency == Frequency.Lower && !isUpperWeek);

                if (!fires) continue;

                var classDate = currentWeekMonday.AddDays((int)dto.Day - 1);

                var classTime = new ClassTime
                {
                    IsUpperWeek = dto.Frequency == Frequency.Both ? null : isUpperWeek,
                    Start = dto.Start,
                    End = dto.End,
                    DaysOfTheWeek = dto.Day,
                    ClassDate = classDate
                };
                classTimes.Add(classTime);

                var classItem = new Class
                {
                    Room = dto.Room,
                    ClassType = dto.ClassType ?? (isLecturer ? ClassType.Лекция : ClassType.Семинар),
                    TaughtSubjectId = taughtSubjectId,
                    ClassTimeId = classTime.Id,
                    ClassTime = classTime
                };
                classes.Add(classItem);

                isLecturer = !isLecturer;
                classesCreated++;
            }

            weekNumber++;
            if (weekNumber > 200) break;
        }

        return (classes, classTimes);
    }

    private static string FormatRange(TimeSpan from, TimeSpan to)
        => $@"{from:hh\:mm} - {to:hh\:mm}";

    private static int GetYear(int firstYear) =>
        DateTime.Now.Month >= 9
            ? firstYear + 1
            : DateTime.Now.Year - firstYear; //2023 => returns current Education Year

    private static int GetAttendanceYear(int year)
        => DateTime.Today.Year - year;

    private static int GetSemester()
        => DateTime.Now.Month >= 9 ? 1 : 2;

    private static int GetAttendanceYearFromAdmission(
        int admissionYear, // the group's acceptance year
        int courseYear) //The student’s current study level within the program
    {
        // courseYear: 1..4
        if (courseYear < 1) courseYear = 1;

        // Academic year start calendar year
        // Example: admission 2023, courseYear 1 -> 2023
        //          admission 2023, courseYear 4 -> 2026
        return admissionYear + (courseYear - 1);
    }
}