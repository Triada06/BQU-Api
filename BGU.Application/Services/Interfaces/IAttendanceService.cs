using BGU.Application.Contracts.Attendances.Requests;
using BGU.Application.Contracts.Attendances.Responses;
using BGU.Core.Entities;

namespace BGU.Application.Services.Interfaces;

public interface IAttendanceService
{
    Task<bool> CreateAsync(CreateAttendanceRequest attendance);
    Task<bool> DeleteAsync(string attendanceId);
    Task<bool> BulkCreateAsync(List<CreateAttendanceRequest> attendances);
    Task<List<Attendance>> GetAllAttendancesAsync();
    Task<UpdateAttendanceResponse> UpdateAttendanceAsync(Attendance attendance);
}