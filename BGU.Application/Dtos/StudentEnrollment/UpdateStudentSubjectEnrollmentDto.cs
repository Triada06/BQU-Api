namespace BGU.Application.Dtos.StudentEnrollment;

public record UpdateStudentSubjectEnrollmentDto(
    string StudentId,
    int Attempt,
    string TaughtSubjectId);