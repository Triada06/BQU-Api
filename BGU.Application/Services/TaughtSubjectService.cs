using System.Linq.Expressions;
using BGU.Application.Contracts.TaughtSubjects.Requests;
using BGU.Application.Contracts.TaughtSubjects.Responses;
using BGU.Application.Dtos.TaughtSubject;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace BGU.Application.Services;

public class TaughtSubjectService(ITaughtSubjectRepository taughtSubjectRepository) : ITaughtSubjectService
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
}