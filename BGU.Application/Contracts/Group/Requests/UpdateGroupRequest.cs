namespace BGU.Application.Contracts.Group.Requests;

public record UpdateGroupRequest(string GroupCode, string SpecializationId, int Year);