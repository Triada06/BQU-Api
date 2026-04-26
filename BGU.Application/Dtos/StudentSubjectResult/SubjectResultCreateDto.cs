namespace BGU.Application.Dtos.StudentSubjectResult;

public sealed record SubjectResultCreateDto(string StudentId, string TaughtSubjectId, double Grade, bool IsFinalised);
