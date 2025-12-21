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
            return new CreateClassTimeResponse(null, StatusCode.BadRequest, false,
                "Start time must be earlier than end time");
        }


        if (request.Days == null || request.Days.Length == 0)
        {
            return new CreateClassTimeResponse(null, StatusCode.BadRequest, false,
                "At least one day must be selected.");
        }

        DaysOfTheWeek combinedDays = DaysOfTheWeek.None;

        foreach (var day in request.Days.Distinct())
            combinedDays |= day;

        bool exists = await classRepository.AnyAsync(ct => ct.Id == request.ClassId &&
                                                           ct.ClassTime.Start == request.Start &&
                                                           ct.ClassTime.End == request.End &&
                                                           ct.ClassTime.DaysOfTheWeek == combinedDays);

        if (exists)
        {
            return new CreateClassTimeResponse(null, StatusCode.BadRequest, false,
                "This class schedule already exists.");
        }


        var classTime = new ClassTime
        {
            Start = request.Start,
            End = request.End,
            DaysOfTheWeek = combinedDays
        };

        if (!await classTimeRepository.CreateAsync(classTime))
        {
            return new CreateClassTimeResponse(null, StatusCode.InternalServerError, false, ResponseMessages.Failed);
        }

        return new CreateClassTimeResponse(classTime, StatusCode.Ok, true, ResponseMessages.Success);
    }
}