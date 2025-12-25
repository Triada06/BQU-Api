using BGU.Application.Contracts.Attendances.Requests;
using BGU.Application.Contracts.Attendances.Responses;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Application.Services;

public class AttendanceService(IAttendanceRepository attendanceRepository) : IAttendanceService
{
    public Task<bool> CreateAsync(CreateAttendanceRequest attendance)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(string attendanceId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> BulkCreateAsync(List<CreateAttendanceRequest> attendances)
    {
        return await attendanceRepository.BulkCreateAsync(attendances.Select(x => new Attendance
        {
            StudentId = x.StudentId,
            ClassId = x.StudentId,
            IsAbsent = false
        }).ToList());
    }

    public Task<List<Attendance>> GetAllAttendancesAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<UpdateAttendanceResponse> UpdateAttendanceAsync(Attendance attendance)
    {
        attendance.IsAbsent = !attendance.IsAbsent;
        return await attendanceRepository.UpdateAsync(attendance)
            ? new UpdateAttendanceResponse(attendance.Id, StatusCode.Ok, true, ResponseMessages.Success)
            : new UpdateAttendanceResponse(attendance.Id, StatusCode.InternalServerError, false,
                "Couldn't update the attendance state");
    }
}