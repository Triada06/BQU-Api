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
        bool exists = await classRepository.AnyAsync(ct =>
            ct.Id == request.ClassId &&
            ct.ClassTime.Start == request.Start &&
            ct.ClassTime.End == request.End &&
            ct.ClassTime.DaysOfTheWeek == request.Day
        );

        if (exists)
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


        if (! await classTimeRepository.CreateAsync(classTime))
        {
            return new CreateClassTimeResponse(
                null,
                StatusCode.InternalServerError,
                false,
                ResponseMessages.Failed
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