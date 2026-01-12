using BGU.Core.Enums;

namespace BGU.Application.Contracts.Group.Requests;

public record CreateGroupRequest(
    string GroupCode,
    string SpecializationId,
    int Year,
    EducationLanguage EducationLanguage,
    EducationLevel EducationLevel);