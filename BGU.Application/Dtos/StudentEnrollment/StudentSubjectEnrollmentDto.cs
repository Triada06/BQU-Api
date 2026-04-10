namespace BGU.Application.Dtos.StudentEnrollment;

public class StudentSubjectEnrollmentDto
{
    public string StudentId { get; set; }
    public string StudentName { get; set; }

    public string TaughtSubjectId { get; set; }
    public string SubjectName { get; set; }

    public string GroupCode { get; set; }

    public int Attempt { get; set; }
}