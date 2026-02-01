namespace BGU.Application.Dtos.Student;

public record GetStudentsForSubject(List<StudentsInSubjectDto> Student);

public sealed record StudentsInSubjectDto(
    string Id,
    string Name,
    string Surname,
    string MiddleName,
    string UserName,
    string GroupName);