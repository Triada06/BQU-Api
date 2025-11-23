namespace BGU.Application.Dtos.TaughtSubject;

public sealed record TaughtSubjectDto(
    string? Id,
    string SubjectId,
    string TeacherId,
    string GroupId,
    string Code,
    int Hours,
    string Operation
);