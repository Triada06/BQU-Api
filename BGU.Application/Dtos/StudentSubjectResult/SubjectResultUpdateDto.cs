namespace BGU.Application.Dtos.StudentSubjectResult;

public sealed record SubjectResultUpdateDto(string StudentId, string TaughtSubjectId, double Grade, bool IsFinalised);