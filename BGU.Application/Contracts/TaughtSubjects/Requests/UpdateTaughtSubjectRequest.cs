namespace BGU.Application.Contracts.TaughtSubjects.Requests;

public sealed record UpdateTaughtSubjectRequest(
    string Code,
    string Title,
    int Credits,
    string DepartmentId,
    string TeacherId,
    string GroupId
);