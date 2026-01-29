using BGU.Core.Enums;

namespace BGU.Application.Dtos.TaughtSubject;

public record GetTaughtSubjectDto(
    string Id,
    string Code,
    string Title,
    string DepartmentName,
    int Year,
    string Teacher,
    string GroupCode,
    int Credits); 