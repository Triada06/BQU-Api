namespace BGU.Application.Dtos.Group;

public record GroupDto(
    string Id,
    string GroupCode,
    string SpecializationName,
    string Language,
    string EducationLevel,
    int Year,
    int StudentCount
);