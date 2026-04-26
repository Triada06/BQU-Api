namespace BGU.Application.Dtos.Student;

public sealed record GetStudentFinals(IEnumerable<GetStudentFinal> Finals);

public sealed record GetStudentFinal(
    string Id,
    string Subject,
    double EnterScore,
    string FormatedDate,
    string TeacherFullName,
    string GroupCode);