using BGU.Core.Enums;

namespace BGU.Application.Dtos.Student;

public sealed record GetActivityAndAttendance(
    string StudentId,
    string StudentFullName,
    List<MangeClassesDto> Classes
);

public sealed record MangeClassesDto(
    string ClassId,
    DateTime ClassDate,
    string FormattedClassHours,
    char ClassType,
    string? AttendanceId,
    bool? IsPresent,
    string? SeminarId,
    Grade? Grade
);