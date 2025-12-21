using BGU.Core.Enums;

namespace BGU.Application.Contracts.Group.Requests;

public record CreateGroupRequest(
    string GroupCode,
    string DepartmentId,
    int Year,
    EducationLanguage EducationLanguage,
    EducationLevel EducationLevel);