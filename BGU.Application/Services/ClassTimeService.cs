using BGU.Application.Contracts.ClassTime.Requests;
using BGU.Application.Contracts.ClassTime.Responses;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Core.Enums;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Application.Services;

public class ClassTimeService(IClassTimeRepository classTimeRepository, IClassRepository classRepository)
    : IClassTimeService
{
    public async Task<CreateClassTimeResponse> CreateAsync(CreateClassTimeRequest request)
    {
        if (request.Start >= request.End)
        {
            return new CreateClassTimeResponse(
                null,
                StatusCode.BadRequest,
                false,
                "Start time must be earlier than end time."
            );
        }

        // Validate day
        if (!Enum.IsDefined(request.Day))
        {
            return new CreateClassTimeResponse(
                null,
                StatusCode.BadRequest,
                false,
                "Invalid day of the week."
            );
        }

        // Check duplicates (same class, same time, same day)
        var @class = (await classRepository.FindAsync(ct =>
                ct.Id == request.ClassId &&
                ct.ClassTime.Start == request.Start &&
                ct.ClassTime.End == request.End &&
                ct.ClassTime.DaysOfTheWeek == request.Day
            , tracking: true)).FirstOrDefault();

        if (@class is null)
        {
            return new CreateClassTimeResponse(
                null,
                StatusCode.BadRequest,
                false,
                "This class schedule already exists."
            );
        }

        var classTime = new ClassTime
        {
            Start = request.Start,
            End = request.End,
            DaysOfTheWeek = request.Day
        };

        @class.ClassTimeId = classTime.Id;

        if (!await classTimeRepository.CreateAsync(classTime))
        {
            return new CreateClassTimeResponse(
                null,
                StatusCode.InternalServerError,
                false,
                ResponseMessages.Failed
            );
        }

        if (!await classRepository.UpdateAsync(@class))
        {
            return new CreateClassTimeResponse(
                null,
                StatusCode.InternalServerError,
                false,
                $"Couldn't update the class with an id of {@class.Id}."
            );
        }

        return new CreateClassTimeResponse(
            classTime,
            StatusCode.Ok,
            true,
            ResponseMessages.Success
        );
    }
}