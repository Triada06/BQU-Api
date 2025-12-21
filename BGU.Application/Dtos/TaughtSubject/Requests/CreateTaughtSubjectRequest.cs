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
    TimeSpan Start,
    TimeSpan End,
    DaysOfTheWeek[] DaysOfTheWeek);