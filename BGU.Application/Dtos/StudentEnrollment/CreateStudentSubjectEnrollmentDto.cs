namespace BGU.Application.Dtos.StudentEnrollment;

public sealed record CreateStudentSubjectEnrollmentDto(
    string StudentId,
    string FailedSubjectCode,
    string TaughtSubjectId,
    int? Attempt);