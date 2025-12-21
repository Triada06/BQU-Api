using System.Linq.Expressions;
using BGU.Application.Contracts.ClassTime.Requests;
using BGU.Application.Contracts.TaughtSubjects.Requests;
using BGU.Application.Contracts.TaughtSubjects.Responses;
using BGU.Application.Dtos.TaughtSubject;
using BGU.Application.Dtos.TaughtSubject.Requests;
using BGU.Application.Dtos.TaughtSubject.Responses;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Core.Enums;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace BGU.Application.Services;

public class TaughtSubjectService(
    ITaughtSubjectRepository taughtSubjectRepository,
    ISubjectRepository subjectRepository,
    IClassTimeService classTimeService,
    IClassRepository classRepository) : ITaughtSubjectService
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
        var classSessions = (int)(taughtSubject.Hours / 1.5);
        var classes = new List<Class>();

        var isLecturer = true;
        for (var i = 0; i < classSessions; i++)
        {
            var classItem = new Class
            {
                ClassType = isLecturer ? ClassType.Лекция : ClassType.Семинар,
                TaughtSubjectId = taughtSubject.Id,
            };
            var classTime = (await classTimeService.CreateAsync(new CreateClassTimeRequest(classItem.Id, request.Start,
                request.End, request.DaysOfTheWeek))).ClassTime;

            if (classTime == null)
            {
                return new CreateTaughtSubjectResponse(null, false, StatusCode.InternalServerError,
                    "Failed to create class time");
            }

            classItem.ClassTimeId = classTime.Id;
            classes.Add(classItem);
            isLecturer = !isLecturer;
        }

        if (!await classRepository.BulkCreateAsync(classes))
        {
            return new CreateTaughtSubjectResponse(null, false, StatusCode.InternalServerError,
                "Something went wrong while creating classes");
        }

        taughtSubject.Classes = classes;
        if (!await taughtSubjectRepository.CreateAsync(taughtSubject))
        {
            return new CreateTaughtSubjectResponse(null, false, StatusCode.InternalServerError,
                "Something went wrong while creating the course");
        }

        return new CreateTaughtSubjectResponse(taughtSubject.Id, true, StatusCode.Ok, ResponseMessages.Success);
    }
}