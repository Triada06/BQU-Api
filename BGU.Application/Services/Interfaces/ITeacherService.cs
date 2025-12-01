using BGU.Application.Contracts.Teacher.Requests;
using BGU.Application.Contracts.Teacher.Responses;

namespace BGU.Application.Services.Interfaces;

public interface ITeacherService
{
    Task<TeacherProfileResponse> GetProfile(string userId);
    Task<TeacherScheduleResponse> GetSchedule(TeacherScheduleRequest request);
    Task<TeacherCoursesResponse> GetCourses(string userId);
    Task<UpdateTeacherResponse> UpdateAsync(string teacherId, UpdateTeacherRequest request);
    Task<DeleteTeacherResponse> DeleteAsync(string teacherId);
    Task<GetByIdTeacherResponse> GetByIdAsync(string teacherId);
    Task<GetAllTeachersResponse> GetAllAsync(int page, int pageSize, bool tracking = false);

}