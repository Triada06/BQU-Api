using BGU.Application.Dtos.Teacher;

namespace BGU.Application.Contracts.Teacher.Responses;

public sealed record TeacherCoursesResponse(
    IEnumerable<TeacherCourseDto>? TeacherCourses,
    string Message,
    bool IsSucceeded,
    int StatusCode);