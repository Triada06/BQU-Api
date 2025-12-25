namespace BGU.Application.Dtos.Subject;

public record SubjectDto(
    string? Id,
    string Name,
    int CreditsNumber,
    string DepartmentId,
    string Operation);