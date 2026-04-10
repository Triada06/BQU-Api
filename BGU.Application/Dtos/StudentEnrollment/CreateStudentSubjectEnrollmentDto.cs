namespace BGU.Application.Dtos.StudentEnrollment;

public class CreateStudentSubjectEnrollmentDto
{
    public string StudentId { get; set; }
    public string TaughtSubjectId { get; set; }
    public int? Attempt { get; set; } // optional → auto-calc if null
}