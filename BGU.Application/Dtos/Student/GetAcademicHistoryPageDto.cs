namespace BGU.Application.Dtos.Student;

public sealed record GetAcademicHistoryPageDto(IEnumerable<GetAcademicHistoryDto> AcademicHistoryDtos);

public sealed record GetAcademicHistoryDto(
    string TaughtSubjectId,
    string TaughtSubjectName,
    string DateFrom,
    string DateTo,
    string ProfessorName);