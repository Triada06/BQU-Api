namespace BGU.Application.Dtos.StudentEnrollment;

public record GetEnrollmentDto(
    string Id,
    string StudentId,
    string StudentFullName,
    string SubjectName,
    string TaughtSubjectId,
    string TaughtSubjectCode);