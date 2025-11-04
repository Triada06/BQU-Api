using BGU.Core.Enums;

namespace BGU.Application.Dtos.Group;

public record GroupDto(
    string? Id,
    string Code,
    string AdmissionYearId,
    EducationLanguage EducationLanguage,
    EducationLevel EducationLevel,
    string SpecializationId,
    string Operation);