using BGU.Application.Common;
using BGU.Application.Contracts.IndependentWorks.Requests;
using BGU.Application.Contracts.IndependentWorks.Responses;
using BGU.Application.Dtos.IndependentWorks;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Application.Services;

public class IndependentWorkService(
    IIndependentWorkRepository independentWorkRepository,
    IStudentRepository studentRepository,
    ITaughtSubjectRepository taughtSubjectRepository) : IIndependentWorkService
{
    public async Task<CreateIndependentWorkResponse> CreateAsync(GradeIndependentWorkRequest request)
    {
        if (!await studentRepository.AnyAsync(x => x.Id == request.StudentId))
        {
            return new CreateIndependentWorkResponse(null, StatusCode.BadRequest, false,
                $"Student with an Id of {request.StudentId} not found. ");
        }

        if (!await taughtSubjectRepository.AnyAsync(x => x.Id == request.TaughtSubjectId))
        {
            return new CreateIndependentWorkResponse(null, StatusCode.Conflict, false,
                $"Subject with an Id of {request.TaughtSubjectId} not found. ");
        }

        if (await independentWorkRepository.AnyAsync(x =>
                x.StudentId == request.StudentId && request.TaughtSubjectId == x.TaughtSubjectId &&
                request.Number != x.Number))
        {
            return new CreateIndependentWorkResponse(null, StatusCode.Conflict, false,
                $"This independent work already exist");
        }

        var independentWork = new IndependentWork
        {
            IsAccepted = false,
            IsConfirmed = false,
            IsPassed = null,
            Number = request.Number,
            StudentId = request.StudentId,
            TaughtSubjectId = request.TaughtSubjectId,
        };
        if (!await independentWorkRepository.CreateAsync(independentWork))
        {
            return new CreateIndependentWorkResponse(null, StatusCode.InternalServerError, false,
                ResponseMessages.Failed);
        }

        return new CreateIndependentWorkResponse(independentWork.Id, StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<DeleteIndependentWorkResponse> DeleteAsync(string id)
    {
        var work = await independentWorkRepository.GetByIdAsync(id, tracking: true);
        if (work is null)
        {
            return new DeleteIndependentWorkResponse(StatusCode.BadRequest, false,
                $"Independent work with and id of {id} not found. ");
        }

        return await independentWorkRepository.DeleteAsync(work)
            ? new DeleteIndependentWorkResponse(StatusCode.Ok, true, ResponseMessages.Success)
            : new DeleteIndependentWorkResponse(StatusCode.InternalServerError, false,
                $"Error while deleting independent work {id}. ");
    }

    public async Task<ApiResult<BulkGradeIndependentWorksDto>> BulkGradeIndependentWorkAsync(
        BulkGradeIndependentWorksDto dto)
    {
        var trueIds = dto.IndependentWorkRequests
            .Where(x => x.IsPassed == true)
            .Select(x => x.IndependentWorkId)
            .ToList();

        var falseIds = dto.IndependentWorkRequests
            .Where(x => x.IsPassed == false)
            .Select(x => x.IndependentWorkId)
            .ToList();

        var nullIds = dto.IndependentWorkRequests
            .Where(x => x.IsPassed == null)
            .Select(x => x.IndependentWorkId)
            .ToList();

        if (trueIds.Count > 0)
            await independentWorkRepository.BulkUpdateAsync(
                x => trueIds.Contains(x.Id),
                s => s.SetProperty(x => x.IsPassed, true));

        if (falseIds.Count > 0)
            await independentWorkRepository.BulkUpdateAsync(
                x => falseIds.Contains(x.Id),
                s => s.SetProperty(x => x.IsPassed, false));

        if (nullIds.Count > 0)
            await independentWorkRepository.BulkUpdateAsync(
                x => nullIds.Contains(x.Id),
                s => s.SetProperty(x => x.IsPassed, (bool?)null));

        return ApiResult<BulkGradeIndependentWorksDto>.Success(dto);
    }
}