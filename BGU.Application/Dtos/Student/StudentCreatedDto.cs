namespace BGU.Application.Dtos.Student;

public class StudentCreatedDto
{
    public string StudentId { get; set; }
    public string UserId { get; set; }
    public string Email { get; set; }
    public string TemporaryPassword { get; set; }
    public string FullName { get; set; }
    public string FINCode { get; set; }
}