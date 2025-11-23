using BGU.Application.Contracts.Teacher.Requests;
using BGU.Application.Contracts.Teacher.Responses;

namespace BGU.Application.Services.Interfaces;

public interface ITeacherService
{
    Task<TeacherProfileResponse> GetProfile(string userId);
    Task<TeacherScheduleResponse> GetSchedule(TeacherScheduleRequest request);
    Task<TeacherCoursesResponse> GetCourses(string userId);
}