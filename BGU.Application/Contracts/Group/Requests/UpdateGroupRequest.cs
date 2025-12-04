namespace BGU.Application.Contracts.Group.Requests;

public record UpdateGroupRequest(string GroupCode, string DepartmentId, int Year, int StudentsCount);