namespace BGU.Application.Dtos.Teacher;

public sealed record TeacherCourseDto(
    string CourseId,
    string CourseName,
    string CourseCode,
    // List<string> Groups,
     string GroupName,
    int CreditCount,
    int StudentCount,
    int Hours);