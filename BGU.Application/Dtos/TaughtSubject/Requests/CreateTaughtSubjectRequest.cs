using BGU.Application.Contracts.ClassTime.Requests;
using BGU.Application.Dtos.Class;
using BGU.Core.Enums;

namespace BGU.Application.Dtos.TaughtSubject.Requests;

public record CreateTaughtSubjectRequest(
    string Code,
    string Title,
    string DepartmentId,
    string TeacherId,
    string GroupId,
    int Credits,
    int Hours,
    CreateClassDto[] ClassTimes,
    int Year,
    int Semster);
