using BGU.Application.Contracts.Student;
using BGU.Application.Contracts.Student.Requests;
using BGU.Application.Contracts.Student.Responses;

namespace BGU.Application.Services.Interfaces;

public interface IStudentService
{
    Task<StudentDashboardResponse> Profile(string userId);
    Task<StudentScheduleResponse> GetSchedule(string userId, StudentScheduleRequest request);
}