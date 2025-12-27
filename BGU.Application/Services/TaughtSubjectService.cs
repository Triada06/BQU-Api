using System.Linq.Expressions;
using BGU.Application.Contracts.Attendances.Requests;
using BGU.Application.Contracts.ClassTime.Requests;
using BGU.Application.Contracts.TaughtSubjects.Requests;
using BGU.Application.Contracts.TaughtSubjects.Responses;
using BGU.Application.Dtos.Class;
using BGU.Application.Dtos.TaughtSubject;
using BGU.Application.Dtos.TaughtSubject.Requests;
using BGU.Application.Dtos.TaughtSubject.Responses;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Core.Enums;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace BGU.Application.Services;

public class TaughtSubjectService(
    ITaughtSubjectRepository taughtSubjectRepository,
    ISubjectRepository subjectRepository,
    IClassTimeService classTimeService,
    IClassTimeRepository classTimeRepository,
    IClassRepository classRepository,
    IStudentRepository studentRepository,
    IAttendanceRepository attendanceRepository,
    IAttendanceService attendanceService) : ITaughtSubjectService
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
            (await taughtSubjectRepository.GetAllAsync(page, pageSize: pageSize,
                include: i => i
                    .Include(x => x.Group)
                    .Include(x => x.Subject)
                    .Include(x => x.Teacher)
                    .ThenInclude(x => x.AppUser),
                tracking: false)).Select(x =>
                new GetTaughtSubjectDto(x.Id, x.Code, x.Subject.Name, x.Teacher.AppUser.Name, x.Group.Code,
                    x.Subject.CreditsNumber));

        return new GetAllTaughtSubjectResponse(subjects, StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<GetByIdTaughtSubjectResponse> GetByIdAsync(string id, bool tracking = false)
    {
        var subject =
            await taughtSubjectRepository.GetByIdAsync(id, include: i => i
                .Include(x => x.Group)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                .ThenInclude(x => x.AppUser), tracking: false);

        if (subject is null)
        {
            return new GetByIdTaughtSubjectResponse(null, StatusCode.NotFound, false, ResponseMessages.NotFound);
        }

        return new GetByIdTaughtSubjectResponse(new GetTaughtSubjectDto(subject.Id, subject.Code, subject.Subject.Name,
            subject.Teacher.AppUser.Name, subject.Group.Code,
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

        if (taughtSubject.Hours % 2 != 0) taughtSubject.Hours += 1;

        var (classes, classTimes) = GenerateClassesAndClassTimes(
            request.Hours,
            request.ClassTimes,
            request.Year,
            request.Semster,
            taughtSubject.Id
        );

        // First create all ClassTimes
        if (!await classTimeRepository.BulkCreateAsync(classTimes))
        {
            return new CreateTaughtSubjectResponse(null, false, StatusCode.InternalServerError,
                "Failed to create class times");
        }

        // Then create all Classes

        if (!await classRepository.BulkCreateAsync(classes))
        {
            return new CreateTaughtSubjectResponse(null, false, StatusCode.InternalServerError,
                "Something went wrong while creating classes");
        }

        taughtSubject.Classes = classes;

        //creating an attendance system
        var studentsInGroup =
            await studentRepository.FindAsync(st => st.StudentAcademicInfo.GroupId == request.GroupId);

        if (studentsInGroup.Count != 0 && studentsInGroup.All(x => x is not null))
        {
            var attendances = new List<Attendance>();

            // For EACH class, create attendance for EACH student
            foreach (var classItem in classes)
            {
                attendances.AddRange(studentsInGroup.Select(student => new Attendance
                    { StudentId = student!.Id, ClassId = classItem.Id, IsAbsent = false }));
            }


            if (!await attendanceRepository.BulkCreateAsync(attendances))
            {
                return new CreateTaughtSubjectResponse(null, false, StatusCode.InternalServerError,
                    "Failed to create attendances");
            }
        }


        return new CreateTaughtSubjectResponse(taughtSubject.Id, true, StatusCode.Ok, ResponseMessages.Success);
    }


    private (List<Class> Classes, List<ClassTime> ClassTimes) GenerateClassesAndClassTimes(
        int hours,
        CreateClassDto[] classDtos,
        int year,
        int semester,
        string taughtSubjectId)
    {
        // Adjust hours if odd
        if (hours % 2 != 0) hours += 1;

        var totalClasses = hours / 2;
        var classes = new List<Class>();
        var classTimes = new List<ClassTime>();

        // Determine semester start date
        var semesterStartDate = semester % 2 == 1
            ? new DateTimeOffset(year, 9, 14, 0, 0, 0, TimeSpan.Zero) // Autumn: Sept 14
            : new DateTimeOffset(year, 2, 1, 0, 0, 0, TimeSpan.Zero); // Spring: Feb 1

        // Find the Monday of the first week (or use start date if it's already Monday)
        var startDayOfWeek = (int)semesterStartDate.DayOfWeek;
        if (startDayOfWeek == 0) startDayOfWeek = 7; // Sunday becomes 7

        var daysUntilMonday = startDayOfWeek == 1 ? 0 : (8 - startDayOfWeek);
        var firstMonday = semesterStartDate.AddDays(daysUntilMonday);

        var isLecturer = true;
        var classesCreated = 0;
        var weekNumber = 1; // Week counter starting from 1

        while (classesCreated < totalClasses)
        {
            var isUpperWeek = weekNumber % 2 == 1; // Week 1, 3, 5... = upper; Week 2, 4, 6... = lower

            // Calculate the Monday of the current week
            var currentWeekMonday = firstMonday.AddDays((weekNumber - 1) * 7);

            // Go through each class schedule for this week
            foreach (var classDto in classDtos)
            {
                if (classesCreated >= totalClasses) break;

                // Check if this class should occur this week based on frequency
                var shouldCreateClass = classDto.Frequency == Frequency.Both ||
                                        (classDto.Frequency == Frequency.Upper && isUpperWeek) ||
                                        (classDto.Frequency == Frequency.Lower && !isUpperWeek);

                if (!shouldCreateClass) continue;

                // Calculate the exact date for this class
                // classDto.Day is 1-5 (Monday-Friday)
                var daysToAdd = (int)classDto.Day - 1; // Monday=1 becomes 0 days to add
                var classDate = currentWeekMonday.AddDays(daysToAdd);

                // Create ClassTime
                var classTime = new ClassTime
                {
                    Id = Guid.NewGuid().ToString(),
                    IsUpperWeek = isUpperWeek,
                    Start = classDto.Start,
                    End = classDto.End,
                    DaysOfTheWeek = classDto.Day,
                    ClassDate = classDate
                };

                classTimes.Add(classTime);

                // Create Class
                var classItem = new Class
                {
                    Id = Guid.NewGuid().ToString(),
                    Room = classDto.Room,
                    ClassType = isLecturer ? ClassType.Лекция : ClassType.Семинар,
                    TaughtSubjectId = taughtSubjectId,
                    ClassTimeId = classTime.Id,
                    ClassTime = classTime
                };

                classes.Add(classItem);

                // Alternate between lecturer and seminar
                isLecturer = !isLecturer;
                classesCreated++;
            }

            // Move to next week
            weekNumber++;
        }

        return (classes, classTimes);
    }
}