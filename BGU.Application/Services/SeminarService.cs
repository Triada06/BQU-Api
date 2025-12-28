using BGU.Application.Contracts.Seminars.Requests;
using BGU.Application.Contracts.Seminars.Responses;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Core.Enums;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Application.Services;

public class SeminarService(
    ISeminarRepository seminarRepository,
    IStudentRepository studentRepository,
    ITaughtSubjectRepository taughtSubjectRepository) : ISeminarService
{
    public async Task<CreateSeminarResponse> CreateAsync(CreateSeminarRequest seminar)
    {
        if (!await studentRepository.AnyAsync(x => x.Id == seminar.StudentId))
        {
            return new CreateSeminarResponse(null, StatusCode.BadRequest, false,
                $"Student with an Id of {seminar.StudentId} not found. ");
        }

        if (!await taughtSubjectRepository.AnyAsync(x => x.Id == seminar.TaughtSubjectId))
        {
            return new CreateSeminarResponse(null, StatusCode.Conflict, false,
                $"Subject with an Id of {seminar.TaughtSubjectId} not found. ");
        }

        if (await seminarRepository.AnyAsync(x =>
                x.StudentId == seminar.StudentId && seminar.TaughtSubjectId == x.TaughtSubjectId))
        {
            return new CreateSeminarResponse(null, StatusCode.Conflict, false,
                $"This seminar already exist");
        }

        var seminarToCreate = new Seminar
        {
            StudentId = seminar.StudentId,
            TaughtSubjectId = seminar.TaughtSubjectId,
        };
        if (!await seminarRepository.CreateAsync(seminarToCreate))
        {
            return new CreateSeminarResponse(null, StatusCode.InternalServerError, false,
                ResponseMessages.Failed);
        }

        return new CreateSeminarResponse(seminarToCreate.Id, StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<DeleteSeminarResponse> DeleteAsync(string id)
    {
        var seminar = await seminarRepository.GetByIdAsync(id, tracking: true);
        if (seminar is null)
        {
            return new DeleteSeminarResponse(StatusCode.BadRequest, false,
                $"Seminar with and id of {id} not found. ");
        }

        return await seminarRepository.DeleteAsync(seminar)
            ? new DeleteSeminarResponse(StatusCode.Ok, true, ResponseMessages.Success)
            : new DeleteSeminarResponse(StatusCode.InternalServerError, false,
                $"Error while deleting seminar {id}. ");
    }
}