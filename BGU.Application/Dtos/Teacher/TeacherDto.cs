using BGU.Core.Enums;

namespace BGU.Application.Dtos.Teacher;

public record TeacherDto(
    string Name,
    string Surname,
    string MiddleName,
    string UserName,
    string DepartmentName,
    TeachingPosition Position
);